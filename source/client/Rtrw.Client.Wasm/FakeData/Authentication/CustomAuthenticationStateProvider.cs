using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Rtrw.Client.Wasm.FakeData.Helpers;
using System.Security.Claims;

namespace Rtrw.Client.Wasm.FakeData.Authentication
{
    #region Mock Claims
    //public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    //{
    //    private readonly ILocalStorageService localStorage;
    //    private ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity());

    //    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    //    {
    //        this.localStorage = localStorage;
    //    }
    //    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        await Task.FromResult(0);
    //        return new AuthenticationState(claimsPrincipal);
    //    }

    //    public void LoginNotify()
    //    {
    //        var identity = new ClaimsIdentity(new[]
    //        {
    //            new Claim(ClaimTypes.Name, "Test"),
    //            new Claim(ClaimTypes.Email, "test@test.com")
    //        }, "Fake Authentication");
    //        claimsPrincipal = new ClaimsPrincipal(identity);
    //        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    //    }

    //    public void LogoutNotify()
    //    {
    //        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    //        claimsPrincipal = anonymous;
    //        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    //    }
    //}
    #endregion
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorageService;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService)
        { this.localStorageService = localStorageService; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorageService.GetItemAsync<string>("token");
            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
            var userClaimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "Fake Authentication"));
            var loginUser = new AuthenticationState(userClaimsPrincipal);
            return loginUser;
        }

        public void Notify() { NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); }
    }
}
