namespace AuthService.Application.Abstractions.Persistence;

public sealed class PersistenceConflictException(
    string conflictCode,
    string message,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    public string ConflictCode { get; } = conflictCode;
}