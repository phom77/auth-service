namespace AuthService.Application.Users.Register;

public enum RegisterStatus
{
    Success = 1,
    InvalidEmail = 2,
    WeakPassword = 3,
    EmailAlreadyExists = 4
}