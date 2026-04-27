using Microsoft.AspNetCore.Mvc.Filters;

namespace DevJob.API.Middleware
{
    public class getUserIdActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

    }
}
