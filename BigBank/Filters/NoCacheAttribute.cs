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
            base.OnResultExecuting(filterContext);
        }
    }
}
