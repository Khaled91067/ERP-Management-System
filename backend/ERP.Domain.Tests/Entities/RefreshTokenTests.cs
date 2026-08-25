namespace ERP.Domain.Tests.Entities;

using System;

using ERP.Domain.Identity.Users;

using FluentAssertions;

using Xunit;

public class RefreshTokenTests
{
    // -----------------------------------------------------------------------
    // IsExpired
    // -----------------------------------------------------------------------

    [Fact]
    public void IsExpired_WhenExpiryIsInTheFuture_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(7));

        // Act & Assert
        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WhenExpiryIsInThePast_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(-1));

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
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(7));

        // Act & Assert
        token.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void IsRevoked_WhenRevoked_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(7));

        // Act
        token.Revoke();

        // Assert
        token.IsRevoked.Should().BeTrue();
    }

    // -----------------------------------------------------------------------
    // IsActive
    // -----------------------------------------------------------------------

    [Fact]
    public void IsActive_WhenNotExpiredAndNotRevoked_ReturnsTrue()
    {
        // Arrange
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(7));

        // Act & Assert
        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_WhenExpired_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenRevoked_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(7));

        // Act
        token.Revoke();

        // Assert
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_WhenExpiredAndRevoked_ReturnsFalse()
    {
        // Arrange
        var token = new RefreshToken("abc123", 1, DateTime.UtcNow.AddDays(-1));

        // Act
        token.Revoke();

        // Assert
        token.IsActive.Should().BeFalse();
    }
}
