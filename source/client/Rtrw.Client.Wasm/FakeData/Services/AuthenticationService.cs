using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Rtrw.Client.Wasm.FakeData.Authentication;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.ViewModels;

namespace Rtrw.Client.Wasm.FakeData.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(LoginRequest loginRequest);

        Task<bool> LogoutAsync();

        Task<bool> CheckIfEmailExistOnDatabase(string email);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApplicationDbContextFactory<SqliteDbContext> dbContextFactory;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ILocalStorageService localStorageService;
        private readonly IAccountLogic accountLogic;

        public AuthenticationService(
            IApplicationDbContextFactory<SqliteDbContext> dbContextFactory,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorageService,
            IAccountLogic accountLogic)
        {
            this.dbContextFactory = dbContextFactory;
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorageService = localStorageService;
            this.accountLogic = accountLogic;
        }

        public async Task<bool> CheckIfEmailExistOnDatabase(string email)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var emailResponse = dbContext.Warga
                .Where(
                    x
                        => x.Email.ToLower() == email.ToLower())
                .FirstOrDefault();
            if (emailResponse != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> LoginAsync(LoginRequest loginRequest)
        {
            var response = await accountLogic.GetAuthenticationTokenAsync(loginRequest);
            if (response == null)
            {
                return false;
            }
            await localStorageService.SetItemAsync("token", response.Token);
            await localStorageService.SetItemAsync("refreshToken", response.RefreshToken);
            (authenticationStateProvider as CustomAuthenticationStateProvider).Notify();

            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            await localStorageService.RemoveItemAsync("token");
            (authenticationStateProvider as CustomAuthenticationStateProvider).Notify();

            return true;
        }
    }
}
