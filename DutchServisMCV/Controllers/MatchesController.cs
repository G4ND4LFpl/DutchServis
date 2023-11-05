using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;

namespace DutchServisMCV.Controllers
{
    public class MatchesController : Controller
    {
        private DutchDatabaseEntities1 database = new DutchDatabaseEntities1();

        public ActionResult Index()
        {
            return RedirectToAction("Tournaments");
        }

        public ActionResult Tournaments()
        {
            // Make query
            var query = from tourn in database.Tournaments
                        where tourn.Type == "tournament"
                        select new TournamentInfo
                        {
                            Name = tourn.Name,
                            Date = tourn.StartDate,
                            Location = tourn.Location,
                            Theme = tourn.Theme,
                            Info = tourn.Info,
                            Img = tourn.ImgPath
                        };

            query = query.OrderByDescending(item => item.Date);

            // Return View
            return View(query);
        }
        

        // GET: Matches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Games games = database.Games.Find(id);
            if (games == null)
            {
                return HttpNotFound();
            }
            return View(games);
        }

        // GET: Matches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Matches/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GameId,MatchId,PointsPlayer1,PointsPlayer2,MistakesPlayer1,MistakesPlayer2,Win,Opening,Dutch")] Games games)
        {
            if (ModelState.IsValid)
            {
                database.Games.Add(games);
                database.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(games);
        }

        // GET: Matches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Games games = database.Games.Find(id);
            if (games == null)
            {
                return HttpNotFound();
            }
            return View(games);
        }

        // POST: Matches/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GameId,MatchId,PointsPlayer1,PointsPlayer2,MistakesPlayer1,MistakesPlayer2,Win,Opening,Dutch")] Games games)
        {
            if (ModelState.IsValid)
            {
                database.Entry(games).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(games);
        }

        // GET: Matches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Games games = database.Games.Find(id);
            if (games == null)
            {
                return HttpNotFound();
            }
            return View(games);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Games games = database.Games.Find(id);
            database.Games.Remove(games);
            database.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
