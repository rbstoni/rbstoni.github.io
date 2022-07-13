using Microsoft.EntityFrameworkCore;
using Rtrw.Client.Wasm.FakeData.JSInterop;

namespace Rtrw.Client.Wasm.FakeData
{
    public interface ISqliteWasmDbContextFactory<TContext> where TContext : DbContext
    {
        Task<TContext> CreateDbContextAsync();
    }

    public class SqliteWasmDbContextFactory<TContext> : ISqliteWasmDbContextFactory<TContext>
        where TContext : DbContext
    {
        private static readonly IDictionary<Type, string> FileNames = new Dictionary<Type, string>();

        private readonly IDbContextFactory<TContext> dbContextFactory;
        private readonly IBrowserCache cache;
        private readonly ISqliteSwap swap;
        private Task<int>? startupTask = null;
        private int lastStatus = -2;
        private bool init = false;

        public SqliteWasmDbContextFactory(
            IDbContextFactory<TContext> dbContextFactory,
            IBrowserCache cache,
            ISqliteSwap swap)
        {
            this.cache = cache;
            this.dbContextFactory = dbContextFactory;
            this.swap = swap;
            startupTask = RestoreAsync();
        }

        private static string Filename => FileNames[typeof(TContext)];
        private static string BackupFile => $"{SqliteWasmDbContextFactory<TContext>.Filename}_bak";

        public static void Reset() => FileNames.Clear();

        public static string? GetFilenameForType() => FileNames.ContainsKey(typeof(TContext)) ? FileNames[typeof(TContext)] : null;

        public async Task<TContext> CreateDbContextAsync()
        {
            await CheckForStartupTaskAsync();
            var dbContext = await dbContextFactory.CreateDbContextAsync();

            if (!init)
            {
                // first time, it should be created
                await dbContext.Database.EnsureCreatedAsync();
                init = true;
            }

            // hook into saved changes
            dbContext.SavedChanges += (o, e) => Ctx_SavedChanges(dbContext, e);

            return dbContext;
        }

        private void DoSwap(string source, string target) => swap.DoSwap(source, target);

        private string GetFilename()
        {
            using var dbContext = dbContextFactory.CreateDbContext();
            var filename = "filenotfound.db";
            var type = dbContext.GetType();
            if (FileNames.ContainsKey(type))
            {
                return FileNames[type];
            }

            var connectionString = dbContext.Database.GetConnectionString();

            if (connectionString != null)
            {
                var file = connectionString.Split(';')
                    .Select(s => s.Split('='))
                    .Select(split => new { key = split[0].ToLowerInvariant(), value = split[1], })
                    .Where(kv => kv.key.Contains("data source") ||
                                 kv.key.Contains("datasource") ||
                                 kv.key.Contains("filename"))
                    .Select(kv => kv.value)
                    .FirstOrDefault();
                if (file != null)
                    filename = file;
            }

            FileNames.Add(type, filename);
            return filename;
        }

        private async Task CheckForStartupTaskAsync()
        {
            if (startupTask != null)
            {
                lastStatus = await startupTask;
                startupTask?.Dispose();
                startupTask = null;
            }
        }

        private async void Ctx_SavedChanges(TContext ctx, SavedChangesEventArgs e)
        {
            await ctx.Database.CloseConnectionAsync();
            await CheckForStartupTaskAsync();
            if (e.EntitiesSavedCount > 0)
            {
                var backupName = $"{SqliteWasmDbContextFactory<TContext>.BackupFile}-{Guid.NewGuid().ToString().Split('-')[0]}";
                DoSwap(SqliteWasmDbContextFactory<TContext>.Filename, backupName);
                lastStatus = await cache.SyncDbWithCacheAsync(backupName);
            }
        }

        private async Task<int> RestoreAsync()
        {
            var filename = $"{GetFilename()}_bak";
            lastStatus = await cache.SyncDbWithCacheAsync(filename);
            if (lastStatus == 0)
            {
                DoSwap(filename, FileNames[typeof(TContext)]);
            }

            return lastStatus;
        }
    }
}
