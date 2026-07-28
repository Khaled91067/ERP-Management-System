namespace ERP.Domain.Identity.Users;

using System;
using ERP.Domain.Shared.Exceptions;

public class RefreshToken
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
    public bool IsRevoked => RevokedAt is not null;
    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken() { }

    public RefreshToken(string token, int userId, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new BusinessRuleValidationException("Refresh token value is required.");

        if (userId <= 0)
            throw new BusinessRuleValidationException("User ID must be valid.");

        Token = token.Trim();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public void Revoke(string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new BusinessRuleValidationException("Refresh token is already revoked.");

        RevokedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(replacedByToken))
        {
            ReplacedByToken = replacedByToken.Trim();
        }
    }
}
