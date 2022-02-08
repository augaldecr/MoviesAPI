using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoviesAPI.Test
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Email, "example@gmail.com"),
                    new Claim(ClaimTypes.Name, "example@gmail.com"),
                    new Claim(ClaimTypes.NameIdentifier, "9992b56a-77ea-4e41-941d-e319b6eb3712"),
                }, "test"));

            await next();
        }
    }
}
