namespace POSsystem.Api.DTOs
{
    public class LoginResponseDto
    {
        // DTOs/LoginResponseDto.cs  
    
            public string Token { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Role { get; set; }
            public DateTime ExpiresAt { get; set; }
        
    }
}
