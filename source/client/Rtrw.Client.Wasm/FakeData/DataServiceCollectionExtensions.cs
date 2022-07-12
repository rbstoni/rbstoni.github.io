using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Rtrw.Client.Wasm.FakeData
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationDbContextFactory<TContext>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            return AddApplicationDbContextFactory<TContext>(
                serviceCollection,
                optionsAction == null ? null : (_, oa) => optionsAction(oa), lifetime);
        }

        public static IServiceCollection AddApplicationDbContextFactory<TContext>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(typeof(IBrowserCache), typeof(BrowserCache), ServiceLifetime.Singleton));

            serviceCollection.TryAdd(
                new ServiceDescriptor(typeof(ISqliteSwap), typeof(SqliteSwap), ServiceLifetime.Singleton));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(IApplicationDbContextFactory<TContext>),
                    typeof(ApplicationDbContextFactory<TContext>),
                    ServiceLifetime.Singleton));

            serviceCollection.AddDbContextFactory<TContext>(
                optionsAction ?? ((s, p) => { }), lifetime);

            return serviceCollection;
        }
    }
}
