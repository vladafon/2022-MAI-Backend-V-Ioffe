using Microsoft.EntityFrameworkCore;
using ProgramEngineering.DB.Models;

namespace ProgramEngineering.DB
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
      : base(options)
        {

        }

        public ApiDbContext()
        {

        }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            new AuthorMap(modelBuilder.Entity<Author>());
            new BookMap(modelBuilder.Entity<Book>());
        }
    }
}