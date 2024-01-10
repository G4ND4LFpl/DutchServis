using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Models;
using System.Data;
using DutchServisMCV.Logic;

namespace DutchServisMCV.Controllers
{
    public class HomeController : DataController
    {
        public ActionResult Index(int? id)
        {
            IQueryable<Announcement> announcements;
            try
            {
                announcements = database.Announcements.OrderByDescending(a => a.Date);
            }
            catch(Exception ex)
            {
                List <Announcement> list = new List<Announcement>();
                list.Add(new Announcement()
                {
                    AnnouncementId = 100,
                    Title = "Błąd serwera",
                    Content = ex.Message,
                    Author = "serwer",
                    Date = DateTime.Now
                });

                return View(list);
            }

            return View(announcements);
        }

        public ActionResult Add()
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View();
        }

        private SResponse Valid(string text, bool hasImage)
        {
            if (hasImage) return new SResponse(true, "");

            if (text == null || text.Replace("\t", "").Replace("\n", "").Replace(" ", "") == "")
            {
                return new SResponse(false, "Nie można opublikować pustego ogłoszenia.");
            }
            else return new SResponse(true, "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Announcement post)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            // Validation
            SResponse response = Valid(post.Content, post.File != null);
            if (!response.Good)
            {
                ViewBag.ContentValidationMsg = response.Message;
                return View();
            }

            response = FileManager.FileExtIsValid(post.File);
            if (!response.Good)
            {
                ViewBag.FileValidationMsg = response.Message;
                return View();
            }

            // Save File
            if (post.File != null)
            {
                string path = Server.MapPath("~/Content/images/announcementdata/") + post.File.FileName;

                try
                {
                    FileManager.Save(post.File, path);
                    post.Img = post.File?.FileName;
                }
                catch (SaveFaildException ex)
                {
                    ViewBag.FileValidationMsg = ex.Message;
                    return View();
                }
            }

            // Prepare model
            post.Author = Session["username"].ToString();

            // Add To Database
            database.Announcements.Add(post);
            database.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Announcement post)
        {
            if (Session["username"] == null) return RedirectToAction("Login", "Admin");

            return View(post);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }

    }
}