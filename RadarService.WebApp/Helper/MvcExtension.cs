using Microsoft.AspNetCore.Mvc.Rendering;

namespace RadarService.WebApp.Helper
{
    public static class MvcExtension
    {
        public static string ActiveClass(this IHtmlHelper htmlHelper, string actions = null, string controllers = null, string cssClass = "active")
        {
            //var currentArea = htmlHelper?.ViewContext.RouteData.DataTokens["area"] as string ?? "";
            var currentController = htmlHelper?.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = htmlHelper?.ViewContext.RouteData.Values["action"] as string;

            //var acceptedArea = (areas ?? currentArea ?? "").Split(',');
            var acceptedControllers = (controllers ?? currentController ?? "").Split(',');
            var acceptedActions = (actions ?? currentAction ?? "").Split(',');

            return acceptedControllers.Contains(currentController) && acceptedActions.Contains(currentAction)
                ? cssClass
                : "";
        }
    }
}
