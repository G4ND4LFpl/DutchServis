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
                defaults: new { controller = "Matches", action = "Tournaments"}
            );

            routes.MapRoute(
                name: "TournamentInfo",
                url: "Tournaments/Info/{name}",
                defaults: new { controller = "Matches", action = "TournamentInfo", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TournamentCreate",
                url: "Tournaments/Create",
                defaults: new { controller = "Matches", action = "TournamentCreate"}
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
                defaults: new { controller = "Matches", action = "Leagues" }
            );

            routes.MapRoute(
                name: "LeagueInfo",
                url: "Leagues/Info/{name}",
                defaults: new { controller = "Matches", action = "LeagueInfo", name = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LeagueCreate",
                url: "Leagues/Create",
                defaults: new { controller = "Matches", action = "LeagueCreate" }
            );

            routes.MapRoute(
                name: "LeagueEdit",
                url: "Leagues/Edit/{name}",
                defaults: new { controller = "Matches", action = "LeagueEdit", name = UrlParameter.Optional }
            );

            // Info

            routes.MapRoute(
                name: "Info",
                url: "Players/Info/{nickname}",
                defaults: new { controller = "Players", action = "Info", nickname = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Edit",
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
