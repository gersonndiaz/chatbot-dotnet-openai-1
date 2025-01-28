using Microsoft.EntityFrameworkCore;

namespace NetOpenAI_1.Entities._Context
{
    public class AppDbSet : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
