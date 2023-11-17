using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DutchServisMCV
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Tournaments

            routes.MapRoute(
                name: "TournInfo",
                url: "Tournaments/Info/{name}",
                defaults: new { controller = "Tournaments", action = "Info", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TournEdit",
                url: "Tournaments/Edit/{name}",
                defaults: new { controller = "Tournaments", action = "Edit", name = UrlParameter.Optional }
            );

            // Leagues

            routes.MapRoute(
                name: "LeagueInfo",
                url: "Leagues/Info/{name}",
                defaults: new { controller = "Leagues", action = "Info", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LeagueEdit",
                url: "Leagues/Edit/{name}",
                defaults: new { controller = "Leagues", action = "Edit", name = UrlParameter.Optional }
            );     

            // Players

            routes.MapRoute(
                name: "PlayerInfo",
                url: "Players/Info/{nickname}",
                defaults: new { controller = "Players", action = "Info", nickname = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PlayerEdit",
                url: "Players/Edit/{nickname}",
                defaults: new { controller = "Players", action = "Edit", nickname = UrlParameter.Optional }
            );

            // Default

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
