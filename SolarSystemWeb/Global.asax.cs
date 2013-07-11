using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Util;

namespace SolarSystemWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private SolarSystem.System.SolarSystem _solarSystem;

        protected void Application_Start()
        {
            HttpEncoder.Current = HttpEncoder.Default;
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _solarSystem = new SolarSystem.System.SolarSystem();
            var t = new Thread(_solarSystem.Run) { Priority = ThreadPriority.Highest, IsBackground = true };
            t.Start();
        }
    }
}