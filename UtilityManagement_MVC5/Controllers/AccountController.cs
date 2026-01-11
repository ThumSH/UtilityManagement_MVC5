using System;
using System.Linq;
using System.Web.Mvc;
using UtilityManagement_MVC5.Models;

namespace UtilityManagement_MVC5.Controllers
{
    public class AccountController : Controller
    {
        // Connect to Database using EF
        private UMS_DBEntities db = new UMS_DBEntities();

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Find user in database
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

            if (user != null)
            {
                // Save info to Session (This is how MVC 5 remembers you)
                Session["UserID"] = user.UserID;
                Session["UserRole"] = user.Role;
                Session["UserName"] = user.FullName;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Credentials";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}