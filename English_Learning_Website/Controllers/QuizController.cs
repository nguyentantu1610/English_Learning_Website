using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using English_Learning_Website.Models;
using PagedList;
using PagedList.Mvc;

namespace English_Learning_Website.Controllers
{
    public class QuizController : Controller
    {
        // GET: Quiz
        English_Learning_WebsiteEntities1 db = new English_Learning_WebsiteEntities1();
        public ActionResult Choose_Image_Quiz()
        {
            List<Vocabulary_Type> vocabulary_Type = db.Vocabulary_Type.Where(s => s.Vocabulary_Type_Code != 6).ToList();
            List<Quiz_Detail> quiz_Details = db.Quiz_Detail.ToList();
            Session["quiz_Details"] = quiz_Details;
            return View(vocabulary_Type);
        }
        static List<Vocabulary> vocabularies = new List<Vocabulary>();
        public ActionResult Do_Image_Quiz(int id)
        {
            vocabularies.Clear();
            Session["Image_Quiz_List"] = 0;
            Session["Image_Quiz_Index"] = 1;
            Session["Image_Quiz_Score"] = 0;
            List<bool> save_Result = new List<bool>();
            Session["Image_Quiz_Result"] = save_Result;
            Session["Vocabulary_Index"] = null;
            List<Vocabulary> vocabulary = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == id).ToList();
            Random random = new Random();
            int x;
            for (int i = 0; i < 5; i++)
            {
                if(vocabularies.Count() < 5)
                {
                    x = random.Next(0, vocabulary.Count());
                    if (vocabularies.Contains(vocabulary[x]))
                    {
                        i--;
                    }
                    else
                    {
                        vocabularies.Add(vocabulary[x]);
                    }
                }
                else
                {
                    break;
                }
            }
            Session["Count_Image_Quiz_List"] = vocabularies.Count();
            if (vocabularies == null || vocabularies.Count() < 5)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
            }
            else
            {
                return View(vocabularies[int.Parse(Session["Image_Quiz_List"].ToString())]);
            }
        }
        [HttpPost]
        public ActionResult Check_Answer(Vocabulary vocabulary, string answer)
        {
            List<bool> save_Result = Session["Image_Quiz_Result"] as List<bool>;
            int list = int.Parse(Session["Image_Quiz_List"].ToString());
            int index = int.Parse(Session["Image_Quiz_Index"].ToString());
            int quiz_Score = int.Parse(Session["Image_Quiz_Score"].ToString());
            Vocabulary vocabulary1 = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == vocabulary.Vocabulary_Code);
            if (vocabulary1 == null)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
            }
            else
            {
                if (vocabulary1.Vocabulary_English.ToLower().Trim() == answer.ToLower().Trim())
                {
                    quiz_Score = quiz_Score + 2;
                    save_Result.Add(true);
                }
                else
                {
                    save_Result.Add(false);
                }
                index++;
                Session["Image_Quiz_Index"] = index;
                list++;
                Session["Image_Quiz_List"] = list;
                Session["Image_Quiz_Result"] = save_Result;
                Session["Image_Quiz_Score"] = quiz_Score;
                if (int.Parse(Session["Image_Quiz_Index"].ToString()) > 5)
                {
                    int user_Code = int.Parse(Session["User_Code"].ToString());
                    if (db.Quiz_Detail.FirstOrDefault(s => s.User_Code == user_Code && s.Vocabulary_Type_Code == vocabulary1.Vocabulary_Type_Code) != null)
                    {
                        Quiz_Detail quiz_Detail = db.Quiz_Detail.FirstOrDefault(s => s.User_Code == user_Code && s.Vocabulary_Type_Code == vocabulary1.Vocabulary_Type_Code);
                        if (quiz_Detail.Quiz_Score < quiz_Score)
                        {
                            quiz_Detail.Quiz_Score = quiz_Score;
                            db.Quiz_Detail.AddOrUpdate(quiz_Detail);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Quiz_Detail quiz_Detail = new Quiz_Detail();
                        quiz_Detail.Quiz_Score = int.Parse(Session["Image_Quiz_Score"].ToString());
                        quiz_Detail.Vocabulary_Type_Code = int.Parse(vocabulary1.Vocabulary_Type_Code.ToString());
                        quiz_Detail.User_Code = int.Parse(Session["User_Code"].ToString());
                        quiz_Detail.Quiz_Code = 6;
                        db.Quiz_Detail.Add(quiz_Detail);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Result", "Quiz");
                }
                else
                {
                    Session["Vocabulary"] = vocabularies[int.Parse(Session["Image_Quiz_List"].ToString())];
                    return RedirectToAction("Continue", "Quiz");
                }
            }
        }
        public ActionResult Continue()
        {
            if (Session["Vocabulary"] == null)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
            }
            else
            {
                Vocabulary vocabulary = Session["Vocabulary"] as Vocabulary;
                return View(vocabulary);
            }
        }
        public ActionResult Result()
        {
            return View();
        }
        public ActionResult Choose_DragDrop_Quiz()
        {
            List<Quiz> quizzes = db.Quizs.Where(s => s.Quiz_Code != 6).ToList();
            List<Quiz_Detail> quiz_Details = db.Quiz_Detail.ToList();
            Session["quiz_Details"] = quiz_Details;
            Session["Choose_DragDrop_Quiz"] = null;
            return View(quizzes);
        }
        public ActionResult Do_DragDrop_Quiz(int id)
        {
            Quiz quizzes = db.Quizs.FirstOrDefault(s => s.Quiz_Code == id);
            Session["Choose_DragDrop_Quiz"] = id;
            return View(quizzes);
        }
        [HttpPost]
        public ActionResult Do_DragDrop_Quiz(string dropone, string droptwo, string dropthree, string dropfour, string dropfive)
        {
            try
            {
                int id = int.Parse(Session["Choose_DragDrop_Quiz"].ToString());
                Quiz quiz = db.Quizs.FirstOrDefault(s => s.Quiz_Code == id);
                int user_Code = int.Parse(Session["User_Code"].ToString());
                Userz userz = db.Userzs.FirstOrDefault(s => s.User_Code == user_Code);
                if(quiz != null && userz != null)
                {
                    int quiz_score = 0;
                    if (dropone == quiz.Quiz_AnswerOne)
                    {
                        quiz_score += 2;
                    }
                    if (droptwo == quiz.Quiz_AnswerTwo)
                    {
                        quiz_score += 2;
                    }
                    if (dropthree == quiz.Quiz_AnswerThree)
                    {
                        quiz_score += 2;
                    }
                    if (dropfour == quiz.Quiz_AnswerFour)
                    {
                        quiz_score += 2;
                    }
                    if (dropfive == quiz.Quiz_AnswerFive)
                    {
                        quiz_score += 2;
                    }
                    Session["DragDrop_Quiz_Score"] = quiz_score;
                    Quiz_Detail quiz_Detail = db.Quiz_Detail.FirstOrDefault(s => s.Quiz_Code == quiz.Quiz_Code && s.User_Code == userz.User_Code);
                    if (quiz_Detail != null)
                    {
                        if (quiz_Detail.Quiz_Score < quiz_score)
                        {
                            quiz_Detail.Quiz_Score = quiz_score;
                            db.Quiz_Detail.AddOrUpdate(quiz_Detail);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Quiz_Detail quiz_Detail1 = new Quiz_Detail();
                        quiz_Detail1.User_Code = userz.User_Code;
                        quiz_Detail1.Quiz_Code = quiz.Quiz_Code;
                        quiz_Detail1.Quiz_Score = quiz_score;
                        quiz_Detail1.Vocabulary_Type_Code = 6;
                        db.Quiz_Detail.AddOrUpdate(quiz_Detail1);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Result_DragDrop_Quiz");
                }
                return this.Do_DragDrop_Quiz(id);
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult Result_DragDrop_Quiz()
        {
            return View();
        }
    }
}