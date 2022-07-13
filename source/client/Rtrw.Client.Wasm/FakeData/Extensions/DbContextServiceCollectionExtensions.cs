using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rtrw.Client.Wasm.FakeData.JSInterop;

namespace Rtrw.Client.Wasm.FakeData.Extensions
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddSqliteWasmDbContextFactory<TContext>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            return serviceCollection.AddSqliteWasmDbContextFactory<TContext>(optionsAction == null ? null : (_, oa) => optionsAction(oa), lifetime);
        }

        public static IServiceCollection AddSqliteWasmDbContextFactory<TContext>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            serviceCollection.TryAdd(new ServiceDescriptor(typeof(IBrowserCache), typeof(BrowserCache), ServiceLifetime.Singleton));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(ISqliteSwap), typeof(SqliteSwap), ServiceLifetime.Singleton));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(ISqliteWasmDbContextFactory<TContext>), typeof(SqliteWasmDbContextFactory<TContext>), ServiceLifetime.Singleton));

            serviceCollection.AddDbContextFactory<TContext>(optionsAction ?? ((s, p) => { }), lifetime);

            return serviceCollection;
        }
    }
}
