using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DutchServisMCV.Logic;
using DutchServisMCV.Logic.GameEngine;

namespace DutchServisMCV.Controllers
{
    public class GameController : Controller
    {
        public static GameEngine engine;

        public ActionResult Index()
        {
            engine = new GameEngine();

            return View();
        }

        [HttpPost]
        public ActionResult Action(string id)
        {
            switch (engine.Mode)
            {
                case GameMode.Deal:
                    {
                        engine.Initialize();

                        return Json(engine.Deal());
                    }
                case GameMode.Lookup:
                    {
                        return Json(engine.Lookup(id));
                    }
                case GameMode.Start:
                    {
                        return Json(engine.Start());
                    }
                case GameMode.Draw:
                    {
                        if (id == "deck") return Json(engine.DrawDeck());
                        else return Json(engine.DrawStack());
                    }
                case GameMode.Throw:
                    {
                        return Json(engine.Throw(id));
                    }
                default:
                    {
                        if (id == "button") return Json(engine.EndTurn());
                        else return Json(engine.Dash(id));
                    }
            }
        }
    }
}