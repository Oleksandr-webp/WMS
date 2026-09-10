using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;

namespace ControllerBasedApi.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}