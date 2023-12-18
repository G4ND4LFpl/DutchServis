using DutchServisMCV.Logic;
using DutchServisMCV.Models;
using DutchServisMCV.Models.GameNamespace;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DutchServisMCV.Controllers
{
    public class ClansController : Controller
    {
        DutchDatabaseEntities database = new DutchDatabaseEntities();

        public ActionResult Index()
        {
            var model = from clans in database.Clans
                        select new ClanInfo
                        {
                            Id = clans.ClanId,
                            Name = clans.Name,
                            Count = (
                            from players in database.Players
                            where players.ClanId == clans.ClanId
                            select players
                            ).Count()
                        };

            return View(model);
        }

        public ActionResult Info(string name)
        {
            if (name == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (database.Clans.Where(item => item.Name == name).FirstOrDefault() == null) return HttpNotFound();

            var clanPlayers = from players in database.Players
                              join clan in database.Clans
                              on players.ClanId equals clan.ClanId
                              where clan.Name == name
                              select new PlayerInfo
                              {
                                  Nickname = players.Nickname,
                                  Img = players.Img,
                                  Rating = players.Rating,
                                  Ranking = (
                                      from set in database.PlayerSet
                                      where set.PlayerId == players.PlayerId
                                      group set by set.PlayerId into gres
                                      select new
                                      {
                                          Id = gres.Key,
                                          Sum = gres.Sum(item => item.RankingGet),
                                      }
                                  ).FirstOrDefault().Sum + players.Rating.Value
                              };

            var model = new ClanInfo
            {
                Id = database.Clans.Where(item => item.Name == name).FirstOrDefault().ClanId,
                Name = name,
                Count = clanPlayers.Count(),
                Players = clanPlayers.ToList()
            };

            return View(model);
        }

        public ActionResult Add()
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View();
        }

        private SResponse NameIsValid(string name, int id = -1)
        {
            if (name == null || name.Replace(" ", "") == "")
            {
                return new SResponse(false, "Pole Nazwa nie może być puste");
            }

            var repetitions = from clans in database.Clans
                              where clans.Name == name.Trim() && clans.ClanId != id
                              select clans;
            if (repetitions.Count() != 0)
            {
                return new SResponse(false, "Pole Nazwa musi być unikalne");
            }

            return new SResponse(true, "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Clans clan)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            SResponse response = NameIsValid(clan.Name);
            if (!response.Good)
            {
                ViewBag.NameValidationMsg = response.Message;
                return View();
            }
            clan.Name = clan.Name.Trim();

            // Add To Database
            database.Clans.Add(clan);
            database.SaveChanges();

            // Redirect to Index
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string name)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Validate adress
            if (name == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Clans clan = database.Clans.Where(item => item.Name == name).FirstOrDefault();
            if (clan == null) return HttpNotFound();

            return View(clan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Clans clan)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            SResponse response = NameIsValid(clan.Name);
            if (!response.Good)
            {
                ViewBag.NameValidationMsg = response.Message;
                return View(clan);
            }
            clan.Name = clan.Name.Trim();

            database.Entry(clan).State = EntityState.Modified;
            database.SaveChanges();

            // Redirect to Info
            return RedirectToAction("Info", new { name = clan.Name });
        }

        public ActionResult Delete(int id)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Validate adress
            if (id == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Clans clan = database.Clans.Find(id);
            if (clan == null) return HttpNotFound();

            // Delete clan
            database.Clans.Remove(clan);

            // Update players
            foreach(Players player in database.Players.Where(p => p.ClanId == id))
            {
                player.ClanId = null;
                database.Entry(player).State = EntityState.Modified;
            }
            database.SaveChanges();

            // Redirect to List
            return RedirectToAction("Index");
        }
    }
}