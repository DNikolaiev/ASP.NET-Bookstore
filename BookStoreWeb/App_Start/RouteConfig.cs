﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookStoreWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{file}.html");
            routes.IgnoreRoute("{file}.jpg");
            routes.IgnoreRoute("{file}.png");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Products" }
            );
            routes.MapRoute(
                name: "Route1",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Add", id = UrlParameter.Optional }

            );
        }
    }
}
