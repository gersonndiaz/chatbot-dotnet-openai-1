using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetOpenAI_1.Entities
{
    [Table("interaction")]
    public class Interaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("chat_id")]
        public int ChatId { get; set; }
        [Column("request")]
        public string Request { get; set; }
        [Column("response")]
        public string Response { get; set; }
        [Column("prompt_tokens")]
        public int PromptTokens { get; set; }
        [Column("completion_tokens")]
        public int CompletionTokens { get; set; }
        [Column("total_tokens")]
        public int TotalTokens { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("modified_at")]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }
    }

}
