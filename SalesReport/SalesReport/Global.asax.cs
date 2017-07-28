using SalesReport.App_Start;
using System.Web.Optimization;
using System.Web.Mvc;
using System.Web.Routing;
using SalesReport.Infrastructure;

namespace SalesReport
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ControllerBuilder.Current.SetControllerFactory(new NinjectContollerFactory());
        }
    }
}
