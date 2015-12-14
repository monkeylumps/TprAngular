using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.WebApi;

namespace KanbanBoardApi.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home
        public ActionResult Error()
        {
            throw new Exception();
        }

        // GET: Home
        public ActionResult ErrorCatch()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                new RaygunClient().SendInBackground(ex);
            }

            return null;
        }
    }
}