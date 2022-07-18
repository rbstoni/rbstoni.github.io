using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Rtrw.Client.Wasm.FakeData;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;

namespace Rtrw.Client.Wasm.Services
{
    public interface ICurrentUser
    {
        Warga Warga { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory;
        public CurrentUser(ISqliteWasmDbContextFactory<SqliteWasmDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public Warga Warga => GetCurrentUser().Result;

        async Task<Warga> GetCurrentUser()
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var warga = dbContext?.Warga?
                .Include(x=>x.Geocoder)
                .FirstOrDefaultAsync(x => x.Phone.Trim().ToLower() == "+6281904084351");

            return await warga;
        }
    }
}
