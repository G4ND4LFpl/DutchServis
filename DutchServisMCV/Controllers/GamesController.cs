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
    public class GamesController : Controller
    {
        private DutchDatabaseEntities db = new DutchDatabaseEntities();

        // GET: Games
        public ActionResult Index()
        {
            return View(db.Tournaments.ToList());
        }

        // GET: Games/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournaments tournaments = db.Tournaments.Find(id);
            if (tournaments == null)
            {
                return HttpNotFound();
            }
            return View(tournaments);
        }

        // GET: Games/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TournamentId,Name,Type,StartDate,EndDate,Location,Theme,Info")] Tournaments tournaments)
        {
            if (ModelState.IsValid)
            {
                db.Tournaments.Add(tournaments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tournaments);
        }

        // GET: Games/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournaments tournaments = db.Tournaments.Find(id);
            if (tournaments == null)
            {
                return HttpNotFound();
            }
            return View(tournaments);
        }

        // POST: Games/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TournamentId,Name,Type,StartDate,EndDate,Location,Theme,Info")] Tournaments tournaments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournaments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tournaments);
        }

        // GET: Games/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tournaments tournaments = db.Tournaments.Find(id);
            if (tournaments == null)
            {
                return HttpNotFound();
            }
            return View(tournaments);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tournaments tournaments = db.Tournaments.Find(id);
            db.Tournaments.Remove(tournaments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
