namespace UserService.DTO
{
    public class UserRegistration
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Audiences { get; set; } 
        
    }
}
