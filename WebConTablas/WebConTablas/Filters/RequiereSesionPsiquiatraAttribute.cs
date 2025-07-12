using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

public class RequiereSesionPsiquiatraAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var idPsiquiatra = session.GetInt32("PsiquiatraId");

        if (idPsiquiatra == null)
        {
            context.Result = new RedirectToActionResult("Login", "Psiquiatras", null);
        }
    }
}
