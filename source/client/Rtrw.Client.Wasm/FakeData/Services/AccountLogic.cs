using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rtrw.Client.Wasm.FakeData.Database;
using Rtrw.Client.Wasm.Models;
using Rtrw.Client.Wasm.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Rtrw.Client.Wasm.FakeData.Services
{
    public interface IAccountLogic
    {
        Task<AuthResponse> GetAuthenticationTokenAsync(LoginRequest login);

        Task<AuthResponse> ActivateTokenUsingRefreshToken(AuthResponse response);
    }

    public class AccountLogic : IAccountLogic
    {
        private readonly IApplicationDbContextFactory<SqliteDbContext> dbContextFactory;
        private readonly TokenSettings options;

        public AccountLogic(
            IApplicationDbContextFactory<SqliteDbContext> dbContextFactory,
            IOptions<TokenSettings> options)
        {
            this.dbContextFactory = dbContextFactory;
            this.options = options.Value;
        }

        public async Task<AuthResponse> ActivateTokenUsingRefreshToken(AuthResponse response)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var tokenHandler = new JwtSecurityTokenHandler();
            var clamsPrincipal = tokenHandler.ValidateToken(
                response.Token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                    ValidateLifetime = true
                },
                out SecurityToken validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            if (jwtToken == null && !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return null;
            }
            var email = clamsPrincipal.Claims
                .Where(
                    x
                        => x.Type == ClaimTypes.Email)
                .Select(
                    x
                        => x.Value)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            Warga? currentWarga = dbContext.Warga
                .Where(
                    x
                        => x.Email.ToLower() == email && x.Token == response.RefreshToken)
                .FirstOrDefault();
            if (currentWarga == null)
            {
                return null;
            }

            return await GetTokenAsync(dbContext, currentWarga, jwtToken.Claims.ToList());
        }

        public async Task<AuthResponse> GetAuthenticationTokenAsync(LoginRequest login)
        {
            using var dbContext = await dbContextFactory.CreateDbContextAsync();
            Warga? currentWarga = dbContext.Warga
                .Where(
                    x
                        => x.Email.ToLower() == login.Email && x.Password == login.Password)
                .FirstOrDefault();
            if (currentWarga != null)
            {
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
                var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
                var userClaims = new List<Claim>()
                {
                    new Claim("email", currentWarga.Email),
                    new Claim("phone", currentWarga.PhoneNumber),
                };
                return await GetTokenAsync(dbContext, currentWarga, userClaims);
            }
            return null!;
        }

        async Task<AuthResponse> GetTokenAsync(SqliteDbContext dbContext, Warga currentWarga, List<Claim> userClaims)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var newJwtToken = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: credentials,
                claims: userClaims);
            string token = new JwtSecurityTokenHandler().WriteToken(newJwtToken);
            var refreshToken = GetRefreshToken();
            currentWarga.Token = refreshToken;
            await dbContext.SaveChangesAsync();
            return new AuthResponse { Token = token, RefreshToken = refreshToken };
        }

        static string GetRefreshToken()
        {
            var key = new Byte[32];
            using (var refreshTokenGenerator = RandomNumberGenerator.Create())
            {
                refreshTokenGenerator.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }
    }
}
