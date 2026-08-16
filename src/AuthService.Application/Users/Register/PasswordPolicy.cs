namespace AuthService.Application.Users.Register;

public static class PasswordPolicy
{
    public const int MinimumLength = 12;
    public const int MaximumLength = 128;

    public static bool IsValid(string? password)
    {
        return password is not null
            && password.Length >= MinimumLength
            && password.Length <= MaximumLength;
    }
}