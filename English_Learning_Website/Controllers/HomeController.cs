using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using English_Learning_Website.Models;

namespace English_Learning_Website.Controllers
{
    public class HomeController : Controller
    {
        English_Learning_WebsiteEntities1 db = new English_Learning_WebsiteEntities1();
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
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null)
            {
                return false;
            }
            string check = @"^((086(\d){7})|(096(\d){7})|(097(\d){7})|(098(\d){7})|(032(\d){7})|(033(\d){7})|(034(\d){7})|(035(\d){7})|(036(\d){7})|(037(\d){7})|(038(\d){7})|(039(\d){7})|(088(\d){7})|(091(\d){7})|(094(\d){7})|(083(\d){7})|(084(\d){7})|(085(\d){7})|(081(\d){7})|(082(\d){7})|(089(\d){7})|(090(\d){7})|(093(\d){7})|(070(\d){7})|(079(\d){7})|(077(\d){7})|(076(\d){7})|(078(\d){7})|(092(\d){7})|(056(\d){7})|(058(\d){7}))$";
            return Regex.IsMatch(phoneNumber.Trim(), check);
        }
        [HttpPost]
        public ActionResult Account(Userz userzBonus)
        {
            try
            {
                Userz userz;
                if(Session["User_Code"] != null)
                {
                    int checkUser = int.Parse(Session["User_Code"].ToString());
                    userz = db.Userzs.FirstOrDefault(s => s.User_Code == checkUser);
                }
                else if(Session["Admin_Code"] != null)
                {
                    int checkAdmin = int.Parse(Session["Admin_Code"].ToString());
                    userz = db.Userzs.FirstOrDefault(s => s.User_Code == checkAdmin);
                }
                else
                {
                    return this.Account();
                }
                if (userz != null)
                {
                    Session["User_FullName"] = userzBonus.User_FullName;
                    userz.User_FullName = userzBonus.User_FullName;
                    userz.User_Gender = userzBonus.User_Gender;
                    userz.User_Birthday = DateTime.Parse(userzBonus.User_Birthday.ToString());
                    Userz user1 = db.Userzs.FirstOrDefault(s => s.User_Mail == userzBonus.User_Mail && s.User_Code != userz.User_Code);
                    if (user1 == null)
                    {
                        userz.User_Mail = userzBonus.User_Mail;
                    }
                    else
                    {
                        ViewData["GmailFound"] = "Gmail existed";
                        return this.Account();
                    }
                    if (IsValidPhoneNumber(userzBonus.User_PhoneNumber))
                    {
                        userz.User_PhoneNumber = userzBonus.User_PhoneNumber;
                    }
                    else
                    {
                        ViewData["PhoneNumberNotValid"] = "Phone Number not valid";
                        return this.Account();
                    }
                    if(userzBonus.User_Image != null)
                    {
                        userz.User_Image = userzBonus.User_Image;
                        Session["User_Image"] = userzBonus.User_Image;
                    }
                    db.Userzs.AddOrUpdate(userz);
                    db.SaveChanges();
                }
                return this.Account();
            }
            catch
            {
                return HttpNotFound();
            }
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
            Session.RemoveAll();
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