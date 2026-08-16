namespace AuthService.Api.Contracts.Auth;

public sealed record RegisterResponse(
    Guid Id,
    string Email);