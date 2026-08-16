using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace AuthService.Domain.Users
{
    public sealed class User
    {
        public Guid Id { get; private set; }

        public string Email { get; private set; } = null!;

        public string NormalizedEmail { get; private set; } = null!;

        public string PasswordHash { get; private set; } = null!;

        public UserStatus Status { get; private set; }

        public DateTimeOffset CreatedAtUtc { get; private set; }

        public DateTimeOffset? UpdatedAtUtc { get; private set; }

        private User()
        {
        }

        private User(
            Guid id,
            string email,
            string normalizedEmail,
            string passwordHash,
            UserStatus status,
            DateTimeOffset createdAtUtc)
        {
            Id = id;
            Email = email;
            NormalizedEmail = normalizedEmail;
            PasswordHash = passwordHash;
            Status = status;
            CreatedAtUtc = createdAtUtc;
        }

        public static User Create(
            EmailAddress email,
            string passwordHash,
            DateTimeOffset createdAtUtc)
        {
            ArgumentNullException.ThrowIfNull(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

            if (createdAtUtc.Offset != TimeSpan.Zero)
            {
                throw new ArgumentException(
                    "Creation time must be in UTC.",
                    nameof(createdAtUtc));
            }

            return new User(
                Guid.NewGuid(),
                email.Value,
                email.NormalizedValue,
                passwordHash,
                UserStatus.Active,
                createdAtUtc);
        }
    }
}
