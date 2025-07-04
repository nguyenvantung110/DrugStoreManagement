﻿namespace drug_store_api.dtos.Users
{
    public class UserDto
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? FullName { get; set; }

        public string Role { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}