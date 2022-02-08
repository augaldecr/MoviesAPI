using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoviesAPI.Data;
using System.Linq;
using System.Security.Claims;

namespace MoviesAPI.Test
{
    public class TestBase
    {
        protected string defaultUserId = "9992b56a-77ea-4e41-941d-e319b6eb3712";
        protected string defaultUserEmail = "example@gmail.com";

        protected ApplicationDbContext BuildContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName).Options;

            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }

        protected ControllerContext BuildControllerContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, defaultUserEmail),
                new Claim(ClaimTypes.Email, defaultUserEmail),
                new Claim(ClaimTypes.NameIdentifier, defaultUserId)
            }));

            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        public WebApplicationFactory<Program> BuildWebApplicationFactory(string dbName, bool ignoreSecurity = true)
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        var descriptorDBContext = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                        if (descriptorDBContext != null)
                        {
                            services.Remove(descriptorDBContext);
                        }

                        services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(dbName));

                        if (ignoreSecurity)
                        {
                            services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();

                            services.AddControllers(options =>
                            {
                                options.Filters.Add(new FakeUserFilter());
                            });
                        }
                    });
                });

            return application;
        }
    }
}