using System.ComponentModel.DataAnnotations;
using AuthService.Application.Users.Register;

namespace AuthService.Api.Contracts.Auth;

public sealed record RegisterRequest(
    [param: Required]
    [param: EmailAddress]
    [param: MaxLength(320)]
    string Email,

    [param: Required]
    [param: StringLength(
        PasswordPolicy.MaximumLength,
        MinimumLength = PasswordPolicy.MinimumLength)]
    string Password);