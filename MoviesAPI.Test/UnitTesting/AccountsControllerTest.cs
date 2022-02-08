using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MoviesAPI.Controllers;
using MoviesAPI.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Test.UnitTesting
{
    [TestClass]
    public class AccountsControllerTest : TestBase
    {
        [TestMethod]
        public async Task CreateUser()
        {
            var dbName = Guid.NewGuid().ToString();
            await CreateUserHelper(dbName);
            var context2 = BuildContext(dbName);
            var count = await context2.Users.CountAsync();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task UserCantLoginIn()
        {
            var dbName = Guid.NewGuid().ToString();
            await CreateUserHelper(dbName);

            var controller = BuildAccountsController(dbName);
            var userInfo = new UserInfo() { Email = "example@gmail.com", Password = "wrongPassword" };
            var response = await controller.Login(userInfo);

            Assert.IsNull(response.Value);
            var result = response.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UserCanLogin()
        {
            var dbName = Guid.NewGuid().ToString();
            await CreateUserHelper(dbName);

            var controller = BuildAccountsController(dbName);
            var userInfo = new UserInfo() { Email = "example@gmail.com", Password = "Aa123456!" };
            var response = await controller.Login(userInfo);
            Assert.IsNotNull(response.Value);
            Assert.IsNotNull(response.Value.Token);
        }

        private async Task CreateUserHelper(string nombreBD)
        {
            var accountsController = BuildAccountsController(nombreBD);
            var userInfo = new UserInfo() { Email = "example@gmail.com", Password = "Aa123456!" };
            await accountsController.CreateUser(userInfo);
        }

        private AccountsController BuildAccountsController(string nombreBD)
        {
            var context = BuildContext(nombreBD);
            var myUserStore = new UserStore<IdentityUser>(context);
            var userManager = BuildUserManager(myUserStore);

            var httpContext = new DefaultHttpContext();
            MockAuth(httpContext);
            var signInManager = SetupSignInManager(userManager, httpContext);

            var myConfig = new Dictionary<string, string>
            {
                {"JWT:key", "ASDJKNKJN4KJ5N4KJSNDKAJSNDAKSJDNK4J5N43KJ53N4KSNDJASKLDNAS" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfig)
                .Build();

            return new AccountsController(userManager, configuration, context, signInManager);
        }

        // Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Shared/MockHelpers.cs
        // Source: https://github.com/dotnet/aspnetcore/blob/master/src/Identity/test/Identity.Test/SignInManagerTest.cs
        // Some code was modified to be adapted to our project.

        private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.AllowedForNewUsers = false;

            options.Setup(o => o.Value).Returns(idOptions);

            var userValidators = new List<IUserValidator<TUser>>();

            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>>();
            pwdValidators.Add(new PasswordValidator<TUser>());

            var userManager = new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return userManager;
        }

        private static SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager,
            HttpContext context, ILogger logger = null, IdentityOptions identityOptions = null,
            IAuthenticationSchemeProvider schemeProvider = null) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(context);
            identityOptions = identityOptions ?? new IdentityOptions();
            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(a => a.Value).Returns(identityOptions);
            var claimsFactory = new UserClaimsPrincipalFactory<TUser>(manager, options.Object);
            schemeProvider = schemeProvider ?? new Mock<IAuthenticationSchemeProvider>().Object;
            var sm = new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory, options.Object, null, schemeProvider, new DefaultUserConfirmation<TUser>());
            sm.Logger = logger ?? (new Mock<ILogger<SignInManager<TUser>>>()).Object;
            return sm;
        }

        private Mock<IAuthenticationService> MockAuth(HttpContext context)
        {
            var auth = new Mock<IAuthenticationService>();
            context.RequestServices = new ServiceCollection().AddSingleton(auth.Object).BuildServiceProvider();
            return auth;
        }
    }
}
