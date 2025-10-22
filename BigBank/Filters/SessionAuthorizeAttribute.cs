using System;
using System.Web;
using System.Web.Mvc;

namespace BigBank.Filters
{
    // Usage: [SessionAuthorize(RolesCsv = "Customer,Manager")]
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {
        public string RolesCsv { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null) return false;
            var session = httpContext.Session;
            if (session == null) return false;

            var userType = session["UserType"] as string;
            if (string.IsNullOrEmpty(userType)) return false;

            if (string.IsNullOrEmpty(RolesCsv)) return true; // any logged in user

            var allowed = RolesCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var r in allowed)
            {
                if (string.Equals(r.Trim(), userType, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirect to login page for unauthorized or not logged in users
            filterContext.Result = new RedirectResult("/Home/Login");
        }
    }
}
