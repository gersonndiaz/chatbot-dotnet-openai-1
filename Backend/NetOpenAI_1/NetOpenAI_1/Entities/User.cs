using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetOpenAI_1.Entities
{
    [Table("user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("google_id")]
        public string GoogleId { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("modified_at")]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        [Column("last_login")]
        public DateTime? LastLogin { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public ICollection<Chat> Chats { get; set; }
    }

}
