using Rtrw.Server.WebApi.Data.Entities;
using Rtrw.Server.WebApi.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Server.WebApi.Services
{
    public interface IAuthenticationService
    {
        string GetAuthenticationToken(LoginRequest login);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext dbContext;

        public AuthenticationService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public string GetAuthenticationToken(LoginRequest login)
        {
            User currentUser = dbContext.Users.Where(x => x.Email == login.Email && x.Password == login.Password).FirstOrDefault();
            return String.Empty;
        }
    }
}
