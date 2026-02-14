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

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">User login credentials.</param>
    /// <returns>JWT token and user information.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Invalid credentials or inactive account.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null ||
            string.IsNullOrWhiteSpace(loginDto.Username) ||
            string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return BadRequest("Invalid request data.");
        }

        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            // Plain text password comparison (MVP only)
            if (user.Password != loginDto.Password)
                return Unauthorized("Invalid credentials.");

            if (!user.IsActive)
                return Unauthorized("Account is inactive.");

            if (!int.TryParse(_config["Jwt:ExpireMinutes"], out int expireMinutes))
                return StatusCode(StatusCodes.Status500InternalServerError, "Invalid JWT configuration.");

            var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

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
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private string GenerateJwtToken(User user, DateTime expiresAt)
    {
        var jwtKey = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey) ||
            string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("JWT configuration is missing.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
