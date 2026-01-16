using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using POSsystem.Api.Models;
using POSsystem.Api.DTOs;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly PosDbContext _context;

    public AuthController(IConfiguration config, PosDbContext context)
    {
        _config = config;
        _context = context;
    }

    // POST: api/auth/login
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        // 1. Find user (username only)
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

        if (user == null)
            return Unauthorized("Invalid credentials");

        // 2. TEMP: plain password comparison (MVP only)
        if (user.Password != loginDto.Password)
            return Unauthorized("Invalid credentials");

        // 3. Check active status
        if (!user.IsActive)
            return Unauthorized("Account is inactive");

        // 4. Generate JWT
        var expiresAt = DateTime.UtcNow.AddMinutes(
            int.Parse(_config["Jwt:ExpireMinutes"])
        );

        var token = GenerateJwtToken(user, expiresAt);

        return Ok(new LoginResponseDto
        {
            Token = token,
            UserId = user.UserId,
            Username = user.UserName,
            Role = user.Role.RoleName,
            ExpiresAt = expiresAt
        });
    }

    private string GenerateJwtToken(User user, DateTime expiresAt)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
