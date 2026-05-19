namespace PCBuilder.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; } // "Admin" lub "User"
        public required string ApiToken { get; set; } // Do punktu z REST API
    }
}
