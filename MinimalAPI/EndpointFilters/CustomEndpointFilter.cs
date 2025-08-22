
namespace MinimalAPI.EndpointFilters
{
    public class CustomEndpointFilter: IEndpointFilter
    {
        private readonly ILogger<CustomEndpointFilter> _logger;

        public CustomEndpointFilter(ILogger<CustomEndpointFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            _logger.LogInformation("Endpoint filter - before logic");

            var result = await next(context);

            _logger.LogInformation("Endpoint filter - after logic");

            return result;
        }
    }
}
