using Microsoft.Extensions.DependencyInjection.Extensions;
using Rtrw.Client.Wasm.Components.Services;
using Rtrw.Client.Wasm.Components.Services.Scroll;

namespace Rtrw.Client.Wasm.Components.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRtrwComponentServices(this IServiceCollection services, ServiceConfiguration configuration = null)
        {
            configuration ??= new ServiceConfiguration();
            return services
                .AddJsApi()
                .AddRtrwModal()
                .AddRtrwPopoverService(configuration.PopoverOptions)
                .AddRtrwScrollManager();
        }
        public static IServiceCollection AddRtrwComponentServices(this IServiceCollection services, Action<ServiceConfiguration> configuration)
        {
            if(configuration==null) throw new ArgumentNullException(nameof(configuration));
            var options = new ServiceConfiguration();
            configuration(options);
            return services
                .AddJsApi()
                .AddRtrwModal()
                .AddRtrwPopoverService(options.PopoverOptions)
                .AddRtrwScrollManager();
        }

        public static IServiceCollection AddJsApi(this IServiceCollection services)
        {
            services.TryAddTransient<IJsApiService, JsApiService>();
            return services;
        }
        public static IServiceCollection AddRtrwModal(this IServiceCollection services)
        {
            services.TryAddScoped<IModalService, ModalService>();

            return services;
        }
        public static IServiceCollection AddRtrwPopoverService(this IServiceCollection services, Action<PopoverOptions> options)
        {
            services.Configure(options);
            services.TryAddScoped<IRtrwPopoverService, RtrwPopoverService>();
            return services;
        }
        public static IServiceCollection AddRtrwPopoverService(this IServiceCollection services, PopoverOptions options)
        {
            options ??= new PopoverOptions();
            services.AddRtrwPopoverService(
                option =>
                {
                    option = options;
                });
            return services;
        }
        public static IServiceCollection AddRtrwScrollManager(this IServiceCollection services)
        {
            services.TryAddTransient<IScrollManager, ScrollManager>();
            return services;
        }

    }
}
