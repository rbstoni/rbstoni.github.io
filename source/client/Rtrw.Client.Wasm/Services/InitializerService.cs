using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;
using Rtrw.Client.Wasm.FakeData;
using Rtrw.Client.Wasm.FakeData.Bogus;

namespace Rtrw.Client.Wasm.Services
{
    public interface IInitializerService
    {
        Task InitializeFakeRandomPostAsync();

    }

    public class InitializerService : IInitializerService
    {
        private readonly ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory;
        private Dummy Dummy { get; } = new Dummy();

        public InitializerService(ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory)
        { this.dbContextFactory = dbContextFactory; }

        public async Task InitializeFakeRandomPostAsync()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var posts = dbContext.Posts;
            if (posts == null || !posts.Any())
            {
                var randomPost = Dummy.GenerateFakePost();
                await posts!.AddAsync(randomPost);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
