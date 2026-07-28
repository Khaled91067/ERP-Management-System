
namespace ERP.Domain.Identity.Users
{
    public class RefreshToken
    {

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ReplacedByToken { get; set; } 

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

        public bool IsRevoked => RevokedAt is not null;

        public bool IsActive => !IsExpired && !IsRevoked;
    }
}
