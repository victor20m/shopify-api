using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;

namespace ShopifyApp.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            try
            {
                throw context.Exception;
            }
            catch (CustomHttpException ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Result = new ObjectResult(new
                {
                    error = ex.Message,
                    code = ex.ErrorCode
                })
                {
                    StatusCode = ex.ErrorCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Result = new ObjectResult(new
                {
                    error = "Internal server error",
                    code = 500
                })
                {
                    StatusCode = 500
                };
            }
        }
    }
}
