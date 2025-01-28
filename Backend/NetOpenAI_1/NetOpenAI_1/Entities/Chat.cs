using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetOpenAI_1.Entities
{
    [Table("chat")]
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("modified_at")]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        [ForeignKey("UserId")]
        public User User { get; set; }
        public ICollection<Interaction> Interactions { get; set; }
    }

}
