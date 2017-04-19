using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ODBCCore
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {                    
            
            routes.MapRoute("Plugin.Misc.ODBCCore.Configure",
                "Plugins/ODBCCore/Views/Configure",
                new { controller = "ODBCCore", action = "Configure" },
                new[] { "Nop.Plugin.Misc.ODBCCore.Controllers" }
                );
        }
    }
}
