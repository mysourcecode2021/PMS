using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApp.Utils
{
    public class RequestAuthenticationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var curUrl = context.HttpContext.Request.Path.ToString().Split("/");
            var session = context.HttpContext.Session.GetString("UserId");
            if (curUrl[1] != "" && curUrl[1] != "Login" && curUrl[1] != "LoginAD" && curUrl[1] != "AttendanceOnline")
            {
                if (session == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Login", msg = "Expired Session" }));
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
