using System.Web;
using System.Web.Mvc;
using BigBank.Filters;

namespace BigBank
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            
            filters.Add(new SessionAuthorizeAttribute());
        }
    }
}
