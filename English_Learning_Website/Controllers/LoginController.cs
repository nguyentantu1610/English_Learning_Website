using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using English_Learning_Website.Models;

namespace English_Learning_Website.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        English_Learning_WebsiteEntities2 db = new English_Learning_WebsiteEntities2();
        //Sign In
        public ActionResult SignIn()
        {
            Userz userz = new Userz();
            if (Request.Cookies["User_Mail"] != null && Request.Cookies["User_Password"] != null)
            {
                userz.User_Mail = Request.Cookies["User_Mail"].Value;
                userz.User_Password = Request.Cookies["User_Password"].Value;
                Userz userz1 = db.Userzs.Where(s => s.User_Mail == userz.User_Mail && s.User_Password == userz.User_Password).FirstOrDefault();
                if (userz1.User_Category == true)
                {
                    Session["Admin_Code"] = userz1.User_Code;
                }
                else
                {
                    Session["User_Code"] = userz1.User_Code;
                }
                Session["User_FullName"] = userz1.User_FullName;
                return RedirectToAction("HomePage", "Home");    
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult SignIn(UserzBonus userz)
        {
            Userz userz1 = db.Userzs.Where(s => s.User_Password == userz.User_Password && s.User_Mail == userz.User_Mail).FirstOrDefault();
            if (userz1 == null)
            {
                ViewData["NotFound"] = " Wrong gmail or password ";
                return View("SignIn", userz);
            }
            else
            {
                if (userz1.User_Category == true)
                {
                    Session["Admin_Code"] = userz1.User_Code;
                }
                else
                {
                    Session["User_Code"] = userz1.User_Code;
                }
                Session["User_FullName"] = userz1.User_FullName;
                if (userz.RememberMe)
                {
                    HttpCookie httpCookie = new HttpCookie("User_Mail");
                    httpCookie.Expires = DateTime.Now.AddDays(1d);
                    httpCookie.Value = userz.User_Mail;
                    Response.Cookies.Add(httpCookie);
                    HttpCookie httpCookie1 = new HttpCookie("User_Password");
                    httpCookie1.Expires = DateTime.Now.AddDays(1d);
                    httpCookie1.Value = userz.User_Password;
                    Response.Cookies.Add(httpCookie1);
                }
                return RedirectToAction("HomePage", "Home");
            }    
        }
        //Sign Up
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(UserzBonus userz)
        {
            try
            {
                Userz userz2 = db.Userzs.FirstOrDefault(s => s.User_Mail == userz.User_Mail);
                if (userz2 == null)
                {
                    if (userz.User_Password == userz.Check_Password)
                    {
                        Userz userz1 = new Userz();
                        userz1.User_FullName = userz.User_FullName;
                        userz1.User_Password = userz.User_Password;
                        userz1.User_Category = false;
                        userz1.User_Mail = userz.User_Mail;
                        userz1.User_Image = "user.png";
                        db.Userzs.AddOrUpdate(userz1);
                        db.SaveChanges();
                        return RedirectToAction("SignIn", "Login");
                    }
                    else
                    {
                        ViewData["WrongPassword"] = " Please enter the correct password ";
                        return View("SignUp", userz);
                    }
                }    
                else
                {
                    ViewData["Existed"] = " Gmail existed ";
                    return View("SignUp", userz);
                }    
            }
                catch (Exception)
                {
                    return HttpNotFound();
                }
            }
        static int reCode;
        Random rd = new Random();
        public ActionResult SendMail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendMail (UserzBonus userzBonus)
        {
            Userz userz = db.Userzs.FirstOrDefault(s => s.User_Mail == userzBonus.User_Mail);
            if (userz != null)
            {
                MailAddress fromGMail = new MailAddress("garena281215@gmail.com", "HKT2 Company");
                MailAddress toGMail = new MailAddress(userzBonus.User_Mail, "Me");
                reCode = rd.Next(100000, 999999);
                MailMessage Message = new MailMessage()
                {
                    From = fromGMail,
                    Subject = "Change Password",
                    Body = "Your verification code is : " + reCode.ToString(),
                    Priority = MailPriority.High,
                    IsBodyHtml = false
                };
                Message.To.Add(toGMail);
                SmtpClient smtp = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "garena281215@gmail.com",
                        Password = "lcxehypdkkvzyzqh"
                    }
                };
                smtp.Send(Message);
                return RedirectToAction("CheckMail", "Login");
            }    
            else
            {
                ViewData["MailNotFound"] = "Gmail not found";
                return View("SendMail",userzBonus);
            }    
        }
        public ActionResult CheckMail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckMail(UserzBonus userzBonus)
        {
            if(userzBonus.CheckMail == reCode)
            {
                return RedirectToAction("ChangePassword", "Login");
            }  
            else
            {
                ViewData["Error"] = " Wrong verification code";
                 return View();
            }    
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(UserzBonus userzBonus)
        {
            Userz userz = db.Userzs.FirstOrDefault(s => s.User_Mail == userzBonus.User_Mail);
           if (userzBonus.Check_Password == userzBonus.User_Password)
            {
                if (userz != null)
                {
                    userz.User_Password = userzBonus.User_Password;
                    db.Userzs.AddOrUpdate(userz);
                    db.SaveChanges();
                    return RedirectToAction("SignIn", "Login");
                }
                else
                {
                    ViewData["Null"] = " Gmail not found ";
                    return View("ChangePassword",userzBonus);
                }
            }    
           else
            {
                ViewData["NotEqual"] = " Please enter correct infomation ";
                return View("ChangePassword", userzBonus);
            }    
        }
    }
}