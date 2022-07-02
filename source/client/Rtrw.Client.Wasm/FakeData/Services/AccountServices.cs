using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Rtrw.Client.Wasm.ViewModels;

namespace Rtrw.Client.Wasm.FakeData.Services
{
    #region Without Token
    //public interface IAccountService
    //{
    //    bool Login(LoginRequest login);
    //    bool Logout();
    //}
    //public class AccountService : IAccountService
    //{
    //    private readonly AuthenticationStateProvider authenticationStateProvider;
    //    private readonly IAccountLogic accountLogic;
    //    private readonly ILocalStorageService localStorage;

    //    public AccountService(AuthenticationStateProvider authenticationStateProvider, IAccountLogic accountLogic, ILocalStorageService localStorage)
    //    {
    //        this.authenticationStateProvider = authenticationStateProvider;
    //        this.accountLogic = accountLogic;
    //        this.localStorage = localStorage;
    //    }

    //    public bool Login()
    //    {
    //        ((CustomAuthenticationStateProvider)authenticationStateProvider).LoginNotify();

    //        return true;
    //    }

    //    public bool Logout()
    //    {
    //        ((CustomAuthenticationStateProvider)authenticationStateProvider).LogoutNotify();

    //        return true;
    //    }
    //}
    #endregion
    public interface IAccountService
    {
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest loginPasswordRequest);

        Task<bool> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
    }

    public class AccountService : IAccountService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ILocalStorageService localStorageService;
        private readonly IAccountLogic accountLogic;

        public AccountService(
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorageService,
            IAccountLogic accountLogic)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorageService = localStorageService;
            this.accountLogic = accountLogic;
        }

        public Task<bool> ForgotPasswordAsync(ForgotPasswordRequest loginPasswordRequest)
        { throw new NotImplementedException(); }

        public Task<bool> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        { throw new NotImplementedException(); }
    }
}
