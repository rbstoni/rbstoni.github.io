using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Rtrw.Client.Wasm.Components.Extensions;
using Rtrw.Client.Wasm.Components.Services;
using Rtrw.Client.Wasm.FakeData;
using Rtrw.Client.Wasm.FakeData.Authentication;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Extensions
{
    public static class RtrwServiceCollectionExtensions
    {
        public static IServiceCollection AddRtrwServices(this IServiceCollection services)
        {
            services.AddRtrwComponentServices();

            services.AddBlazoredLocalStorage();

            services.AddApplicationDbContextFactory<SqliteDbContext>(options => options.UseSqlite("Data Source=rtrw.db"));
            services.AddScoped<IInitializerService, InitializerService>();
            services.AddScoped<AuthenticationStateProvider, FakeAuthenticationStateProvider>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IWindowHistoryService, WindowHistoryService>();

            return services;
        }
    }
}
