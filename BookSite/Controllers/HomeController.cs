using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookSite.Models;
namespace BookSite.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        public HomeController()
        {
            db = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            if (User.IsInRole("Member"))
                return RedirectToAction("Index", "Member");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}