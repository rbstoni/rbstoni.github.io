using Microsoft.Extensions.DependencyInjection.Extensions;
using Rtrw.Client.Wasm.Components.Services.Scroll;

namespace Rtrw.Client.Wasm.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRtrwScrollManager(this IServiceCollection services)
        {
            services.TryAddTransient<IScrollManager, ScrollManager>();
            return services;
        }

        public static IServiceCollection AddRtrwModal(this IServiceCollection services)
        {
            services.TryAddScoped<IModalService, ModalService>();

            return services;
        }
    }
}
