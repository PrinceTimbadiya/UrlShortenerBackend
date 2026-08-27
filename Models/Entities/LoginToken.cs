using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortenerBackend.Models.Entities
{
    [Table("LoginToken")]
    public class LoginToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string JwtToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsRevoked { get; set; } = false;
    }
}