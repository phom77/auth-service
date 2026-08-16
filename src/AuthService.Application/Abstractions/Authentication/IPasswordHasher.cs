using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Abstractions.Authentication
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        PasswordVerificationResult Verify(
            string passwordHash,
            string providedPassword);
    }
}
