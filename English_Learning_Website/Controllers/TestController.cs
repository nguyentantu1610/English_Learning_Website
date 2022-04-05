using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
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
        static List<TestBonus> testBonus = new List<TestBonus>();
        public ActionResult Do_Test(int id)
        {
            Test test = db.Tests.FirstOrDefault(s => s.Test_Code == id);
            if (test == null)
            {
                return RedirectToAction("Choose_Test", "Test");
            }
            else
            {
                List<Question> questions = db.Questions.Where(s => s.Test_Code == test.Test_Code).ToList();
                if (questions.Count < 16)
                {
                    return RedirectToAction("Choose_Test", "Test");
                }
                else
                {
                    testBonus.Clear();
                    Session["Index_Test"] = 0;
                    Session["Number_Test"] = 1;
                    Session["Test_Score"] = 0;
                    List<bool> save_Result = new List<bool>();
                    Session["save_Answer"] = save_Result;
                    Session["Test"] = null;
                    Session["paragraph"] = test.Test_Paragraph;
                    Session["video"] = test.Test_Video;
                    Random random = new Random();
                    int x;
                    for (int i = 0; i < 17; i ++)
                    {
                        if(testBonus.Count() < 16)
                        {
                            x = random.Next(0, questions.Count());
                            int code = questions[x].Question_Code;
                            Question question = db.Questions.FirstOrDefault(s => s.Question_Code == code);
                            Answer answer = db.Answers.FirstOrDefault(s => s.Question_Code == question.Question_Code);
                            TestBonus testBonus1 = new TestBonus();
                            testBonus1.Question_Code = question.Question_Code;
                            testBonus1.Question_Content = question.Question_Content;
                            testBonus1.Question_Type = question.Question_Type;
                            testBonus1.Answer_Content1 = answer.Answer_Content1;
                            testBonus1.Answer_Content2 = answer.Answer_Content2;
                            testBonus1.Answer_Content3 = answer.Answer_Content3;
                            testBonus1.Answer_Content4 = answer.Answer_Content4;
                            int z = 0;
                            foreach(TestBonus testBonus2 in testBonus)
                            {
                                if(testBonus2.Question_Code == testBonus1.Question_Code)
                                {
                                    z++;
                                }
                            }
                            if(z > 0)
                            {
                                i--;
                            }
                            else
                            {
                                testBonus.Add(testBonus1);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    List<TestBonus> testBonusx = testBonus.Where(s => s.Question_Type == "text").ToList();
                    List<TestBonus> testBonusy = testBonus.Where(s => s.Question_Type == "paragraph").ToList();
                    List<TestBonus> testBonusz = testBonus.Where(s => s.Question_Type == "video").ToList();
                    testBonus.Clear();
                    testBonus.AddRange(testBonusx);
                    testBonus.AddRange(testBonusy);
                    testBonus.AddRange(testBonusz);
                    if (testBonus == null || testBonus.Count() < 16)
                    {
                        return RedirectToAction("Choose_Test", "Test");
                    }
                    else
                    {
                        return View(testBonus[int.Parse(Session["Index_Test"].ToString())]);
                    }
                }
            }
        }
        [HttpPost]
        public ActionResult Check_Answer(TestBonus testBonus1)
        {
            List<bool> save_Answer = Session["save_Answer"] as List<bool>;
            int index = int.Parse(Session["Index_Test"].ToString());
            int no = int.Parse(Session["Number_Test"].ToString());
            int test_Score = int.Parse(Session["Test_Score"].ToString());
            Question question = db.Questions.FirstOrDefault(s => s.Question_Code == testBonus1.Question_Code);
            if (question == null)
            {
                return RedirectToAction("Choose_Test", "Test");
            }
            else
            {
                Answer answer = db.Answers.First(s => s.Question_Code == question.Question_Code);
                if (answer.Answer_Correct == testBonus1.Answer_Selected)
                {
                    test_Score++;
                    save_Answer.Add(true);
                }
                else
                {
                    save_Answer.Add(false);
                }
                no++;
                Session["Number_Test"] = no;
                index++;
                Session["Index_Test"] = index;
                Session["save_Answer"] = save_Answer;
                Session["Test_Score"] = test_Score;
                if (int.Parse(Session["Number_Test"].ToString()) > 16)
                {
                    int user_Code = int.Parse(Session["User_Code"].ToString());

                    if (db.Test_Detail.FirstOrDefault(s => s.User_Code == user_Code && s.Test_Code == question.Test_Code) != null)
                    {
                        Test_Detail test_Detail = db.Test_Detail.FirstOrDefault(s => s.User_Code == user_Code && s.Test_Code == question.Test_Code);
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
                        test_Detail.Test_Code = int.Parse(question.Test_Code.ToString());
                        test_Detail.User_Code = int.Parse(Session["User_Code"].ToString());
                        test_Detail.Test_Score = test_Score;
                        db.Test_Detail.Add(test_Detail);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Result", "Test");
                }
                else
                {
                    Session["Test"] = testBonus[int.Parse(Session["Index_Test"].ToString())];
                    return RedirectToAction("Continue", "Test");
                }
            }
        }
        public ActionResult Continue()
        {
            if (Session["Test"] == null)
            {
                return RedirectToAction("HomePage", "Home");
            }
            else
            {
                TestBonus testBonus = Session["Test"] as TestBonus;
                return View(testBonus);
            }
        }
        public ActionResult Result(TestBonus testBonus)
        {
            
            return View();
        }
    }
}