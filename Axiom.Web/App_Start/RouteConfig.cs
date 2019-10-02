using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Axiom.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Login",
                url: "Login/Index",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ResetPassword",
                url: "Login/ResetPassword",
                defaults: new { controller = "Login", action = "ResetPassword", id = UrlParameter.Optional }
            );

            routes.MapRoute(
           name: "Application",
           url: "{*url}",
           defaults: new { controller = "Home", action = "Index" });

            AreaRegistration.RegisterAllAreas();
        }
    }
}
