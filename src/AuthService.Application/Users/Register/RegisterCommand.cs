namespace AuthService.Application.Users.Register;

public sealed record RegisterCommand(
    string Email,
    string Password);