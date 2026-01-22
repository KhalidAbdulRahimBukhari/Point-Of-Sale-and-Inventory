namespace POSsystem.Api.DTOs
{
    public class CreateUserDto
    {
        // Person
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }

        // User
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public int ShopId { get; set; }
    }

}
