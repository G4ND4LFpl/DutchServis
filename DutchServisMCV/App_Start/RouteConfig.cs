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
                name: "Tournaments",
                url: "Tournaments",
                defaults: new { controller = "Matches", action = "Tournaments", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TournamentInfo",
                url: "Tournaments/Info/{name}",
                defaults: new { controller = "Matches", action = "TournamentInfo", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TournamentEdit",
                url: "Tournaments/Edit/{name}",
                defaults: new { controller = "Matches", action = "TournamentEdit", name = UrlParameter.Optional }
            );

            // Leagues

            routes.MapRoute(
                name: "Leagues",
                url: "Leagues",
                defaults: new { controller = "Matches", action = "Leagues", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LeagueInfo",
                url: "Leagues/Info/{name}",
                defaults: new { controller = "Matches", action = "LeagueInfo", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LeagueEdit",
                url: "Leagues/Edit/{name}",
                defaults: new { controller = "Matches", action = "LeagueEdit", name = UrlParameter.Optional }
            );

            // Info

            routes.MapRoute(
                name: "Info",
                url: "{controller}/{action}/{name}"
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
