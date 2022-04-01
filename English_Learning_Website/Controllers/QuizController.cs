using System;
using System.Collections.Generic;
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
        // GET: Quiz abc
        English_Learning_WebsiteEntities2 db = new English_Learning_WebsiteEntities2();
        public ActionResult Choose_Quiz()
        {
            List<Vocabulary_Type> vocabulary_Type = db.Vocabulary_Type.ToList();
            return View(vocabulary_Type);
        }
        public ActionResult Do_Quiz(int id)
        {
            Session["Index_List"] = 0;
            Session["No"] = 1;
            Session["Quiz_Score"] = 0;
            List<bool> save_Result = new List<bool>();
            Session["save_Result"] = save_Result;
            List<Vocabulary> vocabulary = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == id).Take(10).ToList();
            Session["Count_List"] = vocabulary.Count();
            if (vocabulary == null || vocabulary.Count() < 10)
            {
                return RedirectToAction("Choose_Quiz", "Quiz");
            }
            else
            {
                return View(vocabulary[int.Parse(Session["Index_List"].ToString())]);
            }
        }
        [HttpPost]
        public ActionResult Check_Answer(Vocabulary vocabulary, string answer)
        {
            List<bool> save_Result = Session["save_Result"] as List<bool>;
            int index = int.Parse(Session["Index_List"].ToString());
            int no = int.Parse(Session["No"].ToString());
            Vocabulary vocabulary1 = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == vocabulary.Vocabulary_Code);
            List<Vocabulary> vocabulary2 = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == vocabulary1.Vocabulary_Type_Code).ToList();
            if (vocabulary1 == null)
            {
                return RedirectToAction("Choose_Quiz", "Quiz");
            }
            else
            {
                if (vocabulary1.Vocabulary_English.ToLower().Trim() == answer.ToLower().Trim())
                {
                    int quiz_Score = int.Parse(Session["Quiz_Score"].ToString());
                    quiz_Score++;
                    Session["Quiz_Score"] = quiz_Score;
                    save_Result.Add(true);
                }
                else
                {
                    save_Result.Add(false);
                }
                index++;
                Session["Index_List"] = index;
                no++;
                Session["No"] = no;
                Session["save_Result"] = save_Result;
                if (int.Parse(Session["No"].ToString()) > 10)
                {
                    return RedirectToAction("Result", "Quiz");
                }
                else
                {
                    Session["Vocabulary"] = vocabulary2[int.Parse(Session["Index_List"].ToString())];
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