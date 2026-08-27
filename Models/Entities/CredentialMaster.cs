using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortenerBackend.Models.Entities
{
    [Table("CredentialMaster")]
    public class CredentialMaster
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public long UserId { get; set; }

        public string? ApiKey { get; set; }

        public string? SecretKeyHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsActive { get; set; }

        public UserMaster? UserMaster { get; set; }
    }
}