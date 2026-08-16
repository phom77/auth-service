using System.ComponentModel.DataAnnotations;
using AuthService.Application.Users.Register;

namespace AuthService.Api.Contracts.Auth;

public sealed record RegisterRequest(
    [property: Required]
    [property: EmailAddress]
    [property: MaxLength(320)]
    string Email,

    [property: Required]
    [property: StringLength(
        PasswordPolicy.MaximumLength,
        MinimumLength = PasswordPolicy.MinimumLength)]
    string Password);