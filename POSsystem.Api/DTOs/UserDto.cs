using System.ComponentModel.DataAnnotations;

namespace POSsystem.Api.DTOs
{
    public class UserDto
    {
        // Person - Required fields
        [Required(ErrorMessage = "FullName is required")]
        [MaxLength(200, ErrorMessage = "FullName cannot exceed 200 characters")]
        public string FullName { get; set; } = null!;

        // Person - Optional fields
        [MaxLength(50, ErrorMessage = "NationalNumber cannot exceed 50 characters")]
        public string? NationalNumber { get; set; }

        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        public DateOnly? DateOfBirth { get; set; } // Using DateOnly for date-only values

        public int? CountryID { get; set; }

        [MaxLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
        public string? Gender { get; set; }

        [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }

        [MaxLength(500, ErrorMessage = "ImagePath cannot exceed 500 characters")]
        public string? ImagePath { get; set; }

        // User - Required fields
        [Required(ErrorMessage = "UserName is required")]
        [MaxLength(100, ErrorMessage = "UserName cannot exceed 100 characters")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(256, ErrorMessage = "Password cannot exceed 256 characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "RoleId is required")]
        public int RoleId { get; set; }

        // ShopId is now optional in the DTO since we'll fix it to 1 in the controller
        public int ShopId { get; set; } = 1; // Default to 1 as requested
    }

    public class UserResponseDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string Role { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}