using DutchServisMCV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DutchServisMCV.Controllers
{
    public class DataController : Controller
    {
        protected DutchServisDBEntities database = new DutchServisDBEntities();

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