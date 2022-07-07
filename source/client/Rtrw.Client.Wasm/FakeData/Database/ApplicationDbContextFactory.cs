using Microsoft.EntityFrameworkCore;

namespace Rtrw.Client.Wasm.FakeData
{
    public interface IApplicationDbContextFactory<TContext> where TContext : DbContext
    {
        Task<TContext> CreateDbContextAsync();
    }

    public class ApplicationDbContextFactory<TContext> : IApplicationDbContextFactory<TContext>
        where TContext : DbContext
    {
        private static readonly IDictionary<Type, string> FileNames = new Dictionary<Type, string>();

        private readonly IDbContextFactory<TContext> dbContextFactory;
        private readonly IBrowserCache cache;
        private readonly ISqliteSwap swap;
        private Task<int>? startupTask = null;
        private int lastStatus = -2;
        private bool init = false;

        public ApplicationDbContextFactory(
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
        private static string BackupFile => $"{ApplicationDbContextFactory<TContext>.Filename}_bak";

        public static void Reset() => FileNames.Clear();

        public static string? GetFilenameForType() => FileNames.ContainsKey(typeof(TContext))
            ? FileNames[typeof(TContext)]
            : null;

        public async Task<TContext> CreateDbContextAsync()
        {
            await CheckForStartupTaskAsync();
            var dnContext = await dbContextFactory.CreateDbContextAsync();

            if (!init)
            {
                // first time, it should be created
                await dnContext.Database.EnsureCreatedAsync();
                init = true;
            }

            // hook into saved changes
            dnContext.SavedChanges += (o, e)
                => Ctx_SavedChanges(dnContext, e);

            return dnContext;
        }

        private void DoSwap(string source, string target) => swap.DoSwap(source, target);

        private string GetFilename()
        {
            using var ctx = dbContextFactory.CreateDbContext();
            var filename = "filenotfound.db";
            var type = ctx.GetType();
            if (FileNames.ContainsKey(type))
            {
                return FileNames[type];
            }

            var cs = ctx.Database.GetConnectionString();

            if (cs != null)
            {
                var file = cs.Split(';')
                    .Select(
                        s
                            => s.Split('='))
                    .Select(
                        split
                            => new { key = split[0].ToLowerInvariant(), value = split[1], })
                    .Where(
                        kv
                            => kv.key.Contains("data source") ||
                            kv.key.Contains("datasource") ||
                            kv.key.Contains("filename"))
                    .Select(
                        kv
                            => kv.value)
                    .FirstOrDefault();
                if (file != null)
                {
                    filename = file;
                }
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
                var backupName = $"{ApplicationDbContextFactory<TContext>.BackupFile}-{Guid.NewGuid().ToString().Split('-')[0]}";
                DoSwap(ApplicationDbContextFactory<TContext>.Filename, backupName);
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
