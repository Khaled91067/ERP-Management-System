
namespace ERP.Domain.Tests.Entities;

using ERP.Domain.Identity.Users;

using FluentAssertions;

public class RefreshTokenTests
{
    // -----------------------------------------------------------------------
    // IsExpired
    // -----------------------------------------------------------------------

    [Fact]
    public void IsExpired_WhenExpiryIsInTheFuture_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenExpiryIsInThePast_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-8)
        };

        // Act & Assert
        token.IsExpired.Should().BeTrue();
    }

    // -----------------------------------------------------------------------
    // IsRevoked
    // -----------------------------------------------------------------------

    [Fact]
    public void IsRevoked_WhenRevokedAtIsNull_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = null
        };

        // Act & Assert
        token.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void IsRevoked_WhenRevokedAtIsSet_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow
        };

        // Act & Assert
        token.IsRevoked.Should().BeTrue();
    }

    // -----------------------------------------------------------------------
    // IsActive
    // -----------------------------------------------------------------------

    [Fact]
    public void IsActive_WhenNotExpiredAndNotRevoked_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = null
        };

        // Act & Assert
        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenExpired_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            RevokedAt = null
        };

        // Act & Assert
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenRevoked_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            RevokedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        // Act & Assert
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenExpiredAndRevoked_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken
        {
            Token = "abc123",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            RevokedAt = DateTime.UtcNow.AddDays(-2)
        };

        // Act & Assert
        token.IsActive.Should().BeFalse();
    }
}
