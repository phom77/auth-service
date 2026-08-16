namespace AuthService.Application.Users.Register;

public sealed record RegisterResult(
    RegisterStatus Status,
    Guid? UserId = null,
    string? Email = null)
{
    public static RegisterResult Succeeded(
        Guid userId,
        string email)
    {
        return new RegisterResult(
            RegisterStatus.Success,
            userId,
            email);
    }

    public static RegisterResult InvalidEmail()
    {
        return new RegisterResult(
            RegisterStatus.InvalidEmail);
    }

    public static RegisterResult WeakPassword()
    {
        return new RegisterResult(
            RegisterStatus.WeakPassword);
    }

    public static RegisterResult EmailAlreadyExists()
    {
        return new RegisterResult(
            RegisterStatus.EmailAlreadyExists);
    }
}