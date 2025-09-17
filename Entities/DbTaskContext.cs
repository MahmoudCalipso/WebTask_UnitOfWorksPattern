using Microsoft.EntityFrameworkCore;

namespace WebTask.Entities
{
    public class DbTaskContext : DbContext
    {
       
        public DbTaskContext(DbContextOptions<DbTaskContext> options) : base(options)
        {
        }
        public DbSet<Tasks> Tasks { get; set; }
    }
}
