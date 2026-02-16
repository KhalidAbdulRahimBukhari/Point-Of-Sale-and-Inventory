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
    private const int SHOP_ID = 1;
    private readonly PosDbContext _context;

    public UserController(PosDbContext context)
    {
        _context = context;
    }

    // -------------------- GET ALL USERS --------------------

    [HttpGet]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserResponseDto>>> GetUsers()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.ShopId == SHOP_ID)
            .Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                IsActive = u.IsActive,
                Role = u.Role.RoleName,
                FullName = u.Person.FullName,
                Email = u.Person.Email,
                Phone = u.Person.Phone
            })
            .ToListAsync();

        return Ok(users);
    }

    // -------------------- GET USER BY ID --------------------

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.UserId == id && u.ShopId == SHOP_ID)
            .Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                IsActive = u.IsActive,
                Role = u.Role.RoleName,
                FullName = u.Person.FullName,
                Email = u.Person.Email,
                Phone = u.Person.Phone
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // -------------------- CREATE USER --------------------

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] UserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) ||
            string.IsNullOrWhiteSpace(dto.Password) ||
            string.IsNullOrWhiteSpace(dto.FullName))
        {
            return BadRequest("Username, Password, and FullName are required");
        }

        if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
            return BadRequest("Username already exists");

        var roleExists = await _context.Roles
            .AnyAsync(r => r.RoleId == dto.RoleId);

        if (!roleExists)
            return BadRequest("Invalid role");

        var person = new Person
        {
            FullName = dto.FullName,
            NationalNumber = string.IsNullOrWhiteSpace(dto.NationalNumber) ? null : dto.NationalNumber,
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email,
            DateOfBirth = dto.DateOfBirth,
            CountryId = dto.CountryID,
            Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender,
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone,
            ImagePath = string.IsNullOrWhiteSpace(dto.ImagePath) ? null : dto.ImagePath,
            CreatedAt = DateTime.UtcNow
        };

        _context.People.Add(person);
        await _context.SaveChangesAsync();

        var user = new User
        {
            PersonId = person.PersonId,
            UserName = dto.UserName,
            Password = dto.Password, // still plaintext as requested
            RoleId = dto.RoleId,
            ShopId = SHOP_ID,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.UserId },
            new
            {
                user.UserId,
                user.UserName,
                person.PersonId
            });
    }

    // -------------------- DEACTIVATE USER --------------------

    [HttpPut("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.ShopId == SHOP_ID);

        if (user == null)
            return NotFound();

        if (user.UserId == 1)
            return StatusCode(403, "Cannot deactivate the primary admin user");

        user.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // -------------------- ACTIVATE USER --------------------

    [HttpPut("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == id && u.ShopId == SHOP_ID);

        if (user == null)
            return NotFound();

        user.IsActive = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
