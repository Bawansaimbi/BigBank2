using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BigBank
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Explicit route for DepositWithdraw to avoid accidental 404 due to routing/order
            routes.MapRoute(
                name: "DepositWithdraw",
                url: "Home/DepositWithdraw",
                defaults: new { controller = "Home", action = "DepositWithdraw" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
