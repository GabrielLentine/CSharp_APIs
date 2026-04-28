using Microsoft.AspNetCore.Mvc.Filters;

namespace CatagoloAPI.Filters
{
    // essa classe executará ações especificas antes e depois de um método Action do controlador ser executado
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;

        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // vai executar antes do método Action do controlador
            _logger.LogInformation("### Executando - OnActionExecuting");
            _logger.LogInformation("#################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"Model State: {context.ModelState.IsValid}");
            _logger.LogInformation("#################");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // vai executar depois do método Action do controlador
            _logger.LogInformation("### Executando - OnActionExecuted");
            _logger.LogInformation("#################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"Status Code: {context.HttpContext.Response.StatusCode}");
            _logger.LogInformation("#################");
        }
    }
}
