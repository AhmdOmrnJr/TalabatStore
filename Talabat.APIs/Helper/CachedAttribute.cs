using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helper
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLIveInSeconds;

        public CachedAttribute(int timeToLIveInSeconds)
        {
            _timeToLIveInSeconds = timeToLIveInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var resposne = await responseCacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(resposne))
            {
                var result = new ContentResult()
                {
                    Content = resposne,
                    ContentType = "application/json",
                    StatusCode = 200,
                };

                context.Result = result;

                return;
            }

            var executedActionRessult = await next.Invoke();

            if (executedActionRessult.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult, TimeSpan.FromSeconds(_timeToLIveInSeconds));
            } 
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path);

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}
