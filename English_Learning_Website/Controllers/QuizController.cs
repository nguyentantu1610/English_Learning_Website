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
        public ActionResult Choose_Quiz()
        {
            List<Vocabulary_Type> vocabulary_Type = db.Vocabulary_Type.ToList();
            List<Quiz_Detail> quiz_Details = db.Quiz_Detail.ToList();
            Session["quiz_Details"] = quiz_Details;
            return View(vocabulary_Type);
        }
        static List<Vocabulary> vocabularies = new List<Vocabulary>();
        public ActionResult Do_Quiz(int id)
        {
            vocabularies.Clear();
            Session["Index_List"] = 0;
            Session["No"] = 1;
            Session["Quiz_Score"] = 0;
            List<bool> save_Result = new List<bool>();
            Session["save_Result"] = save_Result;
            Session["Vocabulary"] = null;
            List<Vocabulary> vocabulary = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == id).ToList();
            Random random = new Random();
            int x;
            for (int i = 0; i < 10; i++)
            {
                if(vocabularies.Count() < 10)
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
            Session["Count_List"] = vocabularies.Count();
            if (vocabularies == null || vocabularies.Count() < 10)
            {
                return RedirectToAction("Choose_Quiz", "Quiz");
            }
            else
            {
                return View(vocabularies[int.Parse(Session["Index_List"].ToString())]);
            }
        }
        [HttpPost]
        public ActionResult Check_Answer(Vocabulary vocabulary, string answer)
        {
            List<bool> save_Result = Session["save_Result"] as List<bool>;
            int index = int.Parse(Session["Index_List"].ToString());
            int no = int.Parse(Session["No"].ToString());
            int quiz_Score = int.Parse(Session["Quiz_Score"].ToString());
            Vocabulary vocabulary1 = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == vocabulary.Vocabulary_Code);
            //List<Vocabulary> vocabulary2 = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == vocabulary1.Vocabulary_Type_Code).ToList();
            if (vocabulary1 == null)
            {
                return RedirectToAction("Choose_Quiz", "Quiz");
            }
            else
            {
                if (vocabulary1.Vocabulary_English.ToLower().Trim() == answer.ToLower().Trim())
                {
                    quiz_Score++;
                    save_Result.Add(true);
                }
                else
                {
                    save_Result.Add(false);
                }
                no++;
                Session["No"] = no;
                index++;
                Session["Index_List"] = index;
                Session["save_Result"] = save_Result;
                Session["Quiz_Score"] = quiz_Score;
                if (int.Parse(Session["No"].ToString()) > 10)
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
                        quiz_Detail.Quiz_Score = int.Parse(Session["Quiz_Score"].ToString());
                        quiz_Detail.Vocabulary_Type_Code = int.Parse(vocabulary1.Vocabulary_Type_Code.ToString());
                        quiz_Detail.User_Code = int.Parse(Session["User_Code"].ToString());
                        db.Quiz_Detail.Add(quiz_Detail);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Result", "Quiz");
                }
                else
                {
                    Session["Vocabulary"] = vocabularies[int.Parse(Session["Index_List"].ToString())];
                    return RedirectToAction("Continue", "Quiz");
                }
            }
        }
        public ActionResult Continue()
        {
            if (Session["Vocabulary"] == null)
            {
                return RedirectToAction("HomePage", "Home");
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
    }
}