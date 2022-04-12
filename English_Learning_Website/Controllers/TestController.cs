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
    public class TestController : Controller
    {
        // GET: Test
        English_Learning_WebsiteEntities1 db = new English_Learning_WebsiteEntities1();
        public ActionResult Choose_Test()
        {
            List<Test> tests = db.Tests.ToList();
            List<Test_Detail> test_Details = db.Test_Detail.ToList();
            Session["test_Details"] = test_Details;
            return View(tests);
        }
        static List<Question> questions = new List<Question>();
        public ActionResult Do_Test(int id)
        {
            questions.Clear();
            Session["Test_Score"] = 0;
            Test test = db.Tests.FirstOrDefault(s => s.Test_Code == id);
            List<Question> questions1 = db.Questions.Where(s => s.Test_Code == test.Test_Code).ToList();
            Session["video"] = test.Test_Video;
            Session["paragraph"] = test.Test_Paragraph;
            Random random = new Random();
            int x;
            for (int i = 0; i < 10; i++)
            {
                if (questions.Count() < 10)
                {
                    x = random.Next(0, questions1.Count());
                    if (questions.Contains(questions1[x]))
                    {
                        i--;
                    }
                    else
                    {
                        questions.Add(questions1[x]);
                    }
                }
                else
                {
                    break;
                }
            }
            List<Question> questions2 = new List<Question>();
            List<Question> questions3 = new List<Question>();
            List<Question> questions4 = new List<Question>();
            List<Question> questions5 = new List<Question>();
            foreach (Question item in questions)
            {
                if (item.Question_Type == "multipe-choice")
                {
                    questions2.Add(item);
                }
                else if(item.Question_Type == "text")
                {
                    questions3.Add(item);
                }
                else if (item.Question_Type == "paragraph")
                {
                    questions4.Add(item);
                }
                else
                {
                    questions5.Add(item);
                }
            }
            Session["questions2"] = questions2;
            Session["questions3"] = questions3;
            Session["questions4"] = questions4;
            Session["questions5"] = questions5;
            questions.Clear();
            questions.AddRange(questions2);
            questions.AddRange(questions3);
            questions.AddRange(questions4);
            questions.AddRange(questions5);
            if (questions == null || questions.Count() < 10)
            {
                return RedirectToAction("Choose_Test", "Test");
            }
            else
            {
                return View(questions);
            }
        }
        [HttpPost]
        public ActionResult Do_Test(string choice1, string choice2, string choice3, string choice4, string choice5, string choice6, string choice7, string text1, string text2, string text3)
        {
            int test_Score =  int.Parse(Session["Test_Score"].ToString());
            if(questions[0].Answer_Correct == choice1)
            {
                test_Score++;
            }
            if (questions[1].Answer_Correct == choice2)
            {
                test_Score++;
            }
            if (questions[2].Answer_Correct == choice3)
            {
                test_Score++;
            }
            if (questions[3].Answer_Correct == text1)
            {
                test_Score++;
            }
            if (questions[4].Answer_Correct == text2)
            {
                test_Score++;
            }
            if (questions[5].Answer_Correct == text3)
            {
                test_Score++;
            }
            if (questions[6].Answer_Correct == choice4)
            {
                test_Score++;
            }
            if (questions[7].Answer_Correct == choice5)
            {
                test_Score++;
            }
            if (questions[8].Answer_Correct == choice6)
            {
                test_Score++;
            }
            if (questions[9].Answer_Correct == choice7)
            {
                test_Score++;
            }
            Session["Test_Score"] = test_Score;
            int user_Code = int.Parse(Session["User_Code"].ToString());
            Userz userz = db.Userzs.FirstOrDefault(s => s.User_Code == user_Code);
            int id = int.Parse(questions[0].Test_Code.ToString());
            if (db.Test_Detail.FirstOrDefault(s => s.Test_Code == id && s.User_Code == user_Code) != null)
            {
                Test_Detail test_Detail = db.Test_Detail.FirstOrDefault(s => s.Test_Code == id && s.User_Code == user_Code);
                if (test_Detail.Test_Score < test_Score)
                {
                    test_Detail.Test_Score = test_Score;
                    db.Test_Detail.AddOrUpdate(test_Detail);
                    db.SaveChanges();
                }
            }
            else
            {
                Test_Detail test_Detail = new Test_Detail();
                test_Detail.Test_Score = int.Parse(Session["Test_Score"].ToString());
                test_Detail.User_Code = int.Parse(Session["User_Code"].ToString());
                test_Detail.Test_Code = int.Parse(questions[0].Test_Code.ToString());
                db.Test_Detail.Add(test_Detail);
                db.SaveChanges();
            }
            MailAddress fromGMail = new MailAddress("garena281215@gmail.com", "HKT2 Company");
            MailAddress toGMail = new MailAddress(userz.User_Mail, "Me");
            MailMessage Message = new MailMessage()
            {
                From = fromGMail,
                Subject = "Create Account Successful",
                Body = "Dear " + userz.User_FullName.ToString() + ",\n"
                + "Your answer :\n"
                + questions[0].Question_Content +"\n"
                + "Your choice : " + choice1 + "\n"
                + questions[1].Question_Content + "\n"
                + "Your choice : " + choice2 + "\n"
                + questions[2].Question_Content + "\n"
                + "Your choice : " + choice3 + "\n"
                + questions[3].Question_Content + "\n"
                + "Your choice : " + text1 + "\n"
                + questions[4].Question_Content + "\n"
                + "Your choice : " + text2 + "\n"
                + questions[5].Question_Content + "\n"
                + "Your choice : " + text3 + "\n"
                + questions[6].Question_Content + "\n"
                + "Your choice : " + choice4 + "\n"
                + questions[7].Question_Content + "\n"
                + "Your choice : " + choice5 + "\n"
                + questions[8].Question_Content + "\n"
                + "Your choice : " + choice6 + "\n"
                + questions[9].Question_Content + "\n"
                + "Your choice : " + choice7 + "\n"
                + "Your Score : " + test_Score + "\n\n"
                + "We wish you have fun and progess in your studies,\n" 
                + "HKT2",
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
            return RedirectToAction("Result_Test","Test");
        }
        public ActionResult Result_Test()
        {
            return View();
        }
    }
}