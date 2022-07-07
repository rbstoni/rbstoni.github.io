using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Rtrw.Client.Wasm.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.FakeData.Authentication
{
    public class FakeAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage;

        public FakeAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = new ClaimsIdentity();
            var token = await localStorage.GetItemAsync<AuthResponse>("token");
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token.Token);
            identity = new ClaimsIdentity(jwtSecurityToken.Claims, "Fake Token Authentication");
            var principal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(principal));
        }
        public void Notify() { NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()); }
    }
}
