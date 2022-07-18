using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Enums;
using Rtrw.Client.Wasm.Models;
using System.ComponentModel;

namespace Rtrw.Client.Wasm.FakeData.Database
{
    public class SqliteWasmDbContext : DbContext
    {
        public SqliteWasmDbContext(DbContextOptions<SqliteWasmDbContext> options) : base(options)
        {
        }

        public DbSet<Post>? Posts { get; set; }
        public DbSet<Warga>? Warga { get; set; }
        public DbSet<Comment>? Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Warga>(entity =>
            {
                entity.HasIndex(e => e.Phone).IsUnique();
                entity.HasMany(e => e.Posts).WithOne(e => e.Author).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.Reports).WithOne(e => e.ReportedBy).OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(e => e.Contacts);
            });
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Gender>()
                .HaveConversion<EnumToStringConverter<Gender>>();
            configurationBuilder.Properties<Emoji>()
                .HaveConversion<EnumToStringConverter<Emoji>>();
            configurationBuilder.Properties<Scope>()
                .HaveConversion<EnumToStringConverter<Scope>>();
        }

        public class EnumToStringConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : Enum
        {
            public EnumToStringConverter() : base(v => v.ToString(), v => (TEnum)Enum.Parse(typeof(TEnum), v))
            { }
        }
    }
}
