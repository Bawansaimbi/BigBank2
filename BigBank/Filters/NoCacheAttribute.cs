using System;
using System.Web.Mvc;

namespace BigBank.Filters
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            response.Cache.SetNoStore();
            response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            response.Cache.SetMaxAge(TimeSpan.Zero);
            response.Cache.SetRevalidation(System.Web.HttpCacheRevalidation.AllCaches);
            // Also add headers for proxies
            response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(filterContext);
        }
    }
}
