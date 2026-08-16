using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Abstractions.Authentication
{
    public enum PasswordVerificationResult
    {
        Failed = 0,
        Success = 1,
        SuccessRehashNeeded = 2
    }
}
