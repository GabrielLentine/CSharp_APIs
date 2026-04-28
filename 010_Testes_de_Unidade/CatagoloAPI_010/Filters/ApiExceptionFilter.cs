using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CatagoloAPI.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;   
    }
    
    // é chamado automaticamente quando ocorrer uma exceção não tratada em um HTTP
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Ocorreu um erro não tratado!");

        context.Result = new ObjectResult("Ocorreu um problema ao tratar a sua solicitação!")
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }
}