using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AccountService.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidationFilter : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if(!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult("ModelState is invalid");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}