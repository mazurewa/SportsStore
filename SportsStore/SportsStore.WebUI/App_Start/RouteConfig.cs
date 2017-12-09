using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SportsStore.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: null,
                url: "",
                defaults: new { controller = "Product", action = "List", category = (string)null,  page = 1}
            );

            routes.MapRoute(
               name: null,
               url: "Strona{page}",
               defaults: new
               {
                   controller = "Product",
                   action = "List",
                   category = (string)null
              // } , new { page = @"\d+" }
              ,
                   page = @"\d+"
               }               
           );

            routes.MapRoute(
               name: null,
               url: "{category}",
               defaults: new { controller = "Product", action = "List", page = 1 }
           );

            routes.MapRoute(
              name: null,
              url: "{category}/Strona{page}",
              defaults: new { controller = "Product", action = "List"
              // } , new { page = @"\d+" }
              ,
                  page = @"\d+"
              }
          );

            routes.MapRoute(null, "{controller}/{action}" );
        }
    }
}
