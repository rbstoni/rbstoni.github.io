using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.FakeData;
using Rtrw.Client.Wasm.FakeData.Authentication;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.FakeData.Services;
using Rtrw.Client.Wasm.Services;
using Rtrw.Client.Wasm.ViewModels;

namespace Rtrw.Client.Wasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Configuration.GetValue<TokenSettings>("TokenSettings");

            builder.Services.AddRtrwModal();
            builder.Services.AddRtrwScrollManager();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddApplicationDbContextFactory<SqliteDbContext>(options => options.UseSqlite("Data Source=rtrw.db"));
            builder.Services.AddScoped<IInitializerService, InitializerService>();
            builder.Services.AddScoped<AuthenticationStateProvider, FakeAuthenticationStateProvider>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IWindowHistoryService, WindowHistoryService>();

            // Authorization
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            var host = builder.Build();
            using (var scope = host.Services.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<IInitializerService>().InitializeFakeRandomPostAsync();
            }
            await host.RunAsync();

            //await builder.Build().RunAsync();
        }
    }
}