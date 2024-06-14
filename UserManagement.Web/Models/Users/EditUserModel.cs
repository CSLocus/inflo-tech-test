using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class EditUserModel
{
    public long Id { get; set; }

    [Required]
    public required string Forename { get; set; }

    [Required]
    public required string Surname { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }

    public bool IsActive { get; set; }
}
