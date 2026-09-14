using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using ControllerBasedApi.Models;
using Microsoft.AspNetCore.Identity;
using WMS.Application.DTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    public UsersController(
        DatabaseContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    // GET: api/User
    [HttpGet]
    [EndpointSummary("Returns all users")]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/User/5
    [HttpGet("id/{id}")]
    [EndpointSummary("Returns user by id")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserById(long id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // POST: api/User/login
    [HttpPost("login")]
    [EndpointSummary("Checks user username and password")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> Login(RegisterUserDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);

        if (user == null)
            return Problem(
                title: "Invalid credentials",
                detail: "Wrong username or password",
                statusCode: StatusCodes.Status401Unauthorized
                );

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return Problem(
                title: "Invalid credentials",
                detail: "Wrong username or password",
                statusCode: StatusCodes.Status401Unauthorized
                );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = jwt
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            Id = id,
            Username = username,
            Role = role
        });
    }

    // PUT: api/User/5
    [HttpPut("{id}")]
    [EndpointSummary("Updates user in database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutUser(long id, User user)
    {
        if (id != user.Id)
            return BadRequest(":");

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when ((!UserExists(user.Id)))
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/User
    [HttpPost]
    [EndpointSummary("Inserts user into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<User>> PostUser(RegisterUserDto dto)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);

        if (existingUser != null)
            return Problem(
                title: "Username already exists",
                detail: "Username already exists.",
                statusCode: StatusCodes.Status409Conflict);

        var user = new User
        {
            Username = dto.Username
        };

        user.Password = _passwordHasher.HashPassword(
            user,
            dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = user.Id },
            new
            {
                user.Id,
                user.Username,
                user.Role
            });
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    [EndpointSummary("Deletes user from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(long? id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(long? id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
