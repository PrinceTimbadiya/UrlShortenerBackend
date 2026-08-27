using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortenerBackend.Models.Entities
{
    [Table("UrlMaster")]
    public class UrlMaster
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        // Nullable because external API requests
        // do not require a logged-in user.
        public long? UserId { get; set; }

        public string? LongUrl { get; set; }

        public string? ShortCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool? IsActive { get; set; }

        public UserMaster? UserMaster { get; set; }
    }
}