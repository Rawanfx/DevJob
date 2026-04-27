using DevJob.Application.DTOs.Cvs;
using Serilog;
namespace DevJob.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandeled Exception {message} on {Method} {path} ",
                   ex.Message, context.Request.Method, context.Request.Path);
                //logger.LogError(ex,"Unhandeled Exception {message} on {Method} {path} ",
                //   ex.Message, context.Request.Method,context.Request.Path);
                await HandleException(context, ex);
            }
        }
        private async Task HandleException(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            if (exception is UnauthorizedAccessException)
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            else
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var responseDto = new ResponseDto
             {
                Success = false,
                Message = context.Response.StatusCode== StatusCodes.Status401Unauthorized?exception.Message:"An Error Occured"
            };
          
          await context.Response.WriteAsJsonAsync(responseDto);
        }
    }
}
