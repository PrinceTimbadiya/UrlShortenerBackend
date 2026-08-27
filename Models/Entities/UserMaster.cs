using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortenerBackend.Models.Entities
{
    [Table("UserMaster")]
    public class UserMaster
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public bool? IsActive { get; set; }

        public ICollection<UrlMaster> UrlMasters { get; set; }
                    = new List<UrlMaster>();

        public ICollection<CredentialMaster> CredentialMasters { get; set; }
                    = new List<CredentialMaster>();
    }
}