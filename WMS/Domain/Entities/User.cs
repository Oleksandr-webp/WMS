namespace WMS.Domain.Entities
{
    public enum UserRole
    {
        User,
        Warehouseman,
        Administrator
    }

    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
    }
}