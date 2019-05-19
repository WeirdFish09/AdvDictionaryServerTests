using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using AdvDictionaryServer.DBContext;
using AdvDictionaryServer.Models;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AdvDictionaryServerTests
{
    public class FakeSignInManager : SignInManager<User>
    {
        User User;
        string password;
        public FakeSignInManager(User user, string password)
                : base(new Mock<FakeUserManager>(user).Object,
                     new Mock<IHttpContextAccessor>().Object,
                     new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                     new Mock<IOptions<IdentityOptions>>().Object,
                     new Mock<ILogger<SignInManager<User>>>().Object,
                     new Mock<IAuthenticationSchemeProvider>().Object)
        {
            User = user;
            this.password = password;
        }

        public override Task SignInAsync(User user, bool persistence, string authenticationMethod)
        {
            return Task.FromResult(SignInResult.Success);
        }

        public override Task<SignInResult> PasswordSignInAsync(User user, string password, bool persistence, bool lockoutOnFailure)
        {
            if(user == User && this.password == password)
                return Task.FromResult(SignInResult.Success);
            return Task.FromResult(SignInResult.Failed);
        }

    }



    public class FakeUserManager : UserManager<User>
    {
        User user;
        public FakeUserManager(User user)
            : base(new Mock<IUserStore<User>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<User>>().Object,
              new IUserValidator<User>[0],
              new IPasswordValidator<User>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<User>>>().Object)
        {
            this.user = user;
        }

        public override Task<IdentityResult> CreateAsync(User user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public override Task<User> FindByEmailAsync(string email)
        {
            return Task.FromResult(user);
        }
    }
}
