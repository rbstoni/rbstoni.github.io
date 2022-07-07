using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;
using Rtrw.Client.Wasm.ViewModels;

namespace Rtrw.Client.Wasm.FakeData.Services
{
    public interface IAccountService
    {
        Warga User { get; }
        Task<bool> CheckIfEmailExistOnDatabase(string email);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest loginPasswordRequest);
        Task<bool> LoginAsync(LoginRequest loginRequest);
        Task<bool> LogoutAsync();
        Task<bool> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
    }

    public class AccountService : IAccountService
    {

        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly IApplicationDbContextFactory<SqliteDbContext> dbContextFactory;
        private readonly ILocalStorageService localStorageService;

        public AccountService(IApplicationDbContextFactory<SqliteDbContext> dbContextFactory,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorageService)
        {
            this.dbContextFactory = dbContextFactory;
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorageService = localStorageService;
        }

        public Warga User { get; private set; }

        public async Task<bool> CheckIfEmailExistOnDatabase(string email)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var emailResponse = dbContext.Warga.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
            if (emailResponse != null)
                return true;
            return false;
        }
        public Task<bool> ForgotPasswordAsync(ForgotPasswordRequest loginPasswordRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoginAsync(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            throw new NotImplementedException();
        }

    }
}
