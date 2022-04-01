using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using English_Learning_Website.Models;

namespace English_Learning_Website.Controllers
{
    public class HomeController : Controller
    {
        English_Learning_WebsiteEntities2 db = new English_Learning_WebsiteEntities2();
        //Home
        public ActionResult HomePage()
        {  
            return View();
        }
        //Account
        public ActionResult Account()
        {
            if (Session["User_Code"] != null)
            {
                int check = int.Parse(Session["User_Code"].ToString());
                Userz user = db.Userzs.FirstOrDefault(s => s.User_Code == check);
                return View("Account", user);
            }
            else
            {
                int check = int.Parse(Session["Admin_Code"].ToString());
                Userz user = db.Userzs.FirstOrDefault(s => s.User_Code == check);
                return View("Account", user);
            }
        }
        [HttpPost]
        public ActionResult Account(Userz userzBonus)
        {
            try
            {
                if (Session["User_Code"] != null)
                {
                    int check = int.Parse(Session["User_Code"].ToString());
                    Userz user = db.Userzs.FirstOrDefault(s => s.User_Code == check);
                    user.User_FullName = userzBonus.User_FullName;
                    user.User_Gender = userzBonus.User_Gender;
                    user.User_Birthday = userzBonus.User_Birthday;
                    Userz user1 = db.Userzs.FirstOrDefault(s => s.User_Mail == userzBonus.User_Mail);
                    if (user1 == null)
                    {
                        user.User_Mail = userzBonus.User_Mail;
                    }
                    user.User_PhoneNumber = userzBonus.User_PhoneNumber;
                    user.User_Image = userzBonus.User_Image;
                    db.Userzs.AddOrUpdate(user);
                    db.SaveChanges();
                }
                if (Session["Admin_Code"] != null)
                {
                    int check = int.Parse(Session["Admin_Code"].ToString());
                    Userz user = db.Userzs.FirstOrDefault(s => s.User_Code == check);
                    user.User_FullName = userzBonus.User_FullName;
                    user.User_Gender = userzBonus.User_Gender;
                    user.User_Birthday = userzBonus.User_Birthday;
                    Userz user1 = db.Userzs.FirstOrDefault(s => s.User_Mail == userzBonus.User_Mail);
                    if (user1 == null)
                    {
                        user.User_Mail = userzBonus.User_Mail;
                    }
                    user.User_PhoneNumber = userzBonus.User_PhoneNumber;
                    user.User_Image = userzBonus.User_Image;
                    db.Userzs.AddOrUpdate(user);
                    db.SaveChanges();
                }
            }
            catch
            {
                return HttpNotFound();
            }
            return RedirectToAction("Account","Home");
        }
        public ActionResult Index()
        {
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
        //Log out
        public ActionResult Logout()
        {
            Session.Remove("User_Code");
            Session.Remove("Admin_Code");
            if (Response.Cookies["User_Mail"] != null && Response.Cookies["User_Password"] != null)
            {
                HttpCookie httpCookie = new HttpCookie("User_Mail");
                httpCookie.Expires = DateTime.Now.AddMonths(-1);
                Response.Cookies.Add(httpCookie);
                HttpCookie httpCookie1 = new HttpCookie("User_Password");
                httpCookie1.Expires = DateTime.Now.AddMonths(-1);
                Response.Cookies.Add(httpCookie1);
            }
            return RedirectToAction("SignIn", "Login");
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            else
            {
                file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            }
            return file.FileName;
        }
    }
}