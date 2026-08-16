using AuthService.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

using ApplicationVerificationResult =
    AuthService.Application.Abstractions.Authentication.PasswordVerificationResult;

using IdentityVerificationResult =
    Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace AuthService.Infrastructure.Authentication
{
    internal sealed class AspNetCorePasswordHasher(PasswordHasher<object> passwordHasher) : IPasswordHasher
    {
        private static readonly object PasswordSubject = new();

        public string Hash(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            return passwordHasher.HashPassword(
                PasswordSubject,
                password);
        }

        public ApplicationVerificationResult Verify(string passwordHash, string providedPassword)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
            ArgumentException.ThrowIfNullOrWhiteSpace(providedPassword);

            var result = passwordHasher.VerifyHashedPassword(
                PasswordSubject,
                passwordHash,
                providedPassword);

            return result switch
            {
                IdentityVerificationResult.Success =>
                    ApplicationVerificationResult.Success,

                IdentityVerificationResult.SuccessRehashNeeded =>
                    ApplicationVerificationResult.SuccessRehashNeeded,

                _ => ApplicationVerificationResult.Failed
            };
        }
    }
}
