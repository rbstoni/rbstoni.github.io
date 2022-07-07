using Microsoft.EntityFrameworkCore;
using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.FakeData.Database
{
    public class SqliteDbContext : DbContext
    {
        public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Warga> Warga { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Warga>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Warga>()
                .HasMany(x => x.Posts);
            modelBuilder.Entity<Post>()
                .HasOne(x => x.Author);
            modelBuilder.Entity<Post>()
                .HasMany(x => x.Mentions);
            modelBuilder.Entity<Post>()
                .HasMany(x => x.Comments);
        }
    }
}
