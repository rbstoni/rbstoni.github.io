using Rtrw.Client.Wasm.FakeData.Database;

namespace Rtrw.Client.Wasm.FakeData.Services
{
    public interface IInitializerService
    {
        Task InitializeFakeRandomPostAsync();

        Task InitializeFakeRandomWargaAsync();
    }

    public class InitializerService : IInitializerService
    {
        private readonly IApplicationDbContextFactory<SqliteDbContext> dbContextFactory;
        private Dummy Dummy { get; } = new Dummy();

        public InitializerService(IApplicationDbContextFactory<SqliteDbContext> dbContextFactory)
        { this.dbContextFactory = dbContextFactory; }

        public async Task InitializeFakeRandomPostAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.Posts.Count() == 0)
            {
                var randomPost = Dummy.FakePost;
                await dbContext.Posts.AddAsync(randomPost);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task InitializeFakeRandomWargaAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            if (dbContext.Warga.Count() == 0)
            {
                var randomWarga = Dummy.FakeWarga;
                await dbContext.Warga.AddAsync(randomWarga);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
