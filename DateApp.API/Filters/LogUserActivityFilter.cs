using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DateApp.Core.Contracts.DAL;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Filters
{
    public class LogUserActivityFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultsContext = await next();

            var userId = int.Parse(resultsContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var repo = resultsContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}
