using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSsystem.Api.Models;
using POSsystem.Api.DTOs;

namespace POSsystem.Api.Controllers;

[ApiController]
[Route("api/users")]

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
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.UserName) ||
            string.IsNullOrWhiteSpace(dto.Password) ||
            string.IsNullOrWhiteSpace(dto.FullName))
        {
            return BadRequest("Username, Password, and FullName are required");
        }

        // Username uniqueness
        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            return BadRequest("Username already exists");

        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            // Create person with proper null handling for nullable fields
            var person = new Person
            {
                FullName = dto.FullName,
                NationalNumber = !string.IsNullOrWhiteSpace(dto.NationalNumber) ? dto.NationalNumber : null,
                Email = !string.IsNullOrWhiteSpace(dto.Email) ? dto.Email : null,
                DateOfBirth = dto.DateOfBirth.HasValue ? dto.DateOfBirth.Value : null,
                CountryId = dto.CountryID.HasValue ? dto.CountryID.Value : null,
                Gender = !string.IsNullOrWhiteSpace(dto.Gender) ? dto.Gender : null,
                Phone = !string.IsNullOrWhiteSpace(dto.Phone) ? dto.Phone : null,
                ImagePath = !string.IsNullOrWhiteSpace(dto.ImagePath) ? dto.ImagePath : null,
                CreatedAt = DateTime.UtcNow
            };

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            // Create user with fixed ShopId = 1
            var user = new User
            {
                PersonId = person.PersonId, // Note: Make sure PersonID property name matches your Person entity
                UserName = dto.UserName,
                Password = dto.Password, // TEMP: plaintext (MVP)
                RoleId = dto.RoleId, // Make sure RoleId is provided in the DTO
                ShopId = 1, // Fixed to 1 as requested
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await tx.CommitAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new
            {
                user.UserId,
                user.UserName,
                PersonId = person.PersonId
            });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            // Log the exception here (ex)
            return BadRequest("Failed to create user" + ex.Message);
        }
    }

    /// <summary>
    /// Soft delete user (set inactive).
    /// </summary>
    /// <summary>
    /// Deactivate user (soft delete).
    /// </summary>
    [HttpPut("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Reactivate user.
    /// </summary>
    [HttpPut("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.IsActive = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
