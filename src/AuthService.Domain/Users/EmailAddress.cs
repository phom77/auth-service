using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text;

namespace AuthService.Domain.Users
{
    public sealed class EmailAddress
    {
        public const int MaxLength = 320;
        public string Value { get; }
        public string NormalizedValue { get; }

        private EmailAddress(string value)
        {
            Value = value;
            NormalizedValue = value.ToUpperInvariant();
        }

        public static EmailAddress Create(string value)
        {
            if(!TryCreate(value, out var email))
            {
                throw new ArgumentException("The email  address is invalid,", nameof(value));
            }

            return email;
        }

        public static bool TryCreate(string? value, [NotNullWhen(true)] out EmailAddress? email)
        {
            email = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var trimmedValue = value.Trim();

            if (trimmedValue.Length > MaxLength)
            {
                return false;
            }

            if (!MailAddress.TryCreate(
                trimmedValue,
                out var parsedAddress))
            {
                return false;
            }

            if (!string.Equals(
                parsedAddress.Address,
                trimmedValue,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            email = new EmailAddress(trimmedValue);
            return true;
        }
    }
}
