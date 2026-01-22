using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.Models;
using POSsystem.Api.DTOs;

namespace POSsystem.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly PosDbContext _context;

    public UserController(PosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all users (admin only).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.Person)
            .Include(u => u.Role)
            .Select(u => new
            {
                u.UserId,
                u.UserName,
                u.IsActive,
                Role = u.Role.RoleName,
                Person = new
                {
                    u.Person.FullName,
                    u.Person.Email,
                    u.Person.Phone
                }
            })
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Get user by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Person)
            .Include(u => u.Role)
            .Where(u => u.UserId == id)
            .Select(u => new
            {
                u.UserId,
                u.UserName,
                u.IsActive,
                Role = u.Role.RoleName,
                Person = new
                {
                    u.Person.FullName,
                    u.Person.Email,
                    u.Person.Phone
                }
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Create a new user (creates Person + User).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        // Username uniqueness
        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            return BadRequest("Username already exists");

        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var person = new Person
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            var user = new User
            {
                PersonId = person.PersonId,
                UserName = dto.UserName,
                Password = dto.Password, // TEMP: plaintext (MVP)
                RoleId = dto.RoleId,
                ShopId = dto.ShopId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new
            {
                user.UserId,
                user.UserName
            });
        }
        catch
        {
            await tx.RollbackAsync();
            return BadRequest("Failed to create user");
        }
    }

    /// <summary>
    /// Soft delete user (set inactive).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        user.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
