﻿using System.Web.Mvc;
using System.Web.Routing;

namespace SparqlEnfdPointWebApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controler = "Home", action = "Index" }
            );
        }
    }
}

