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
        // GET: Quiz
        English_Learning_WebsiteEntities1 db = new English_Learning_WebsiteEntities1();
        public ActionResult Choose_Quiz()
        {
            List<Vocabulary_Type> vocabulary_Type = db.Vocabulary_Type.ToList();
            return View(vocabulary_Type);
        }
        //public ActionResult Do_Quiz(int id, int? page)
        //{
        //    if (page == null) page = 1;
        //    int pageSize = 3;
        //    int pageNum = page ?? 1;
        //    List<Vocabulary> vocabulary = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == id).ToList();
        //    if (vocabulary == null)
        //    {
        //        return RedirectToAction("Choose_Quiz", "Quiz");
        //    }
        //    else
        //    {
        //        return View(vocabulary.ToPagedList(pageNum, pageSize));
        //    }
        //}
        public ActionResult Do_Quiz(int id)
        {
            Session["number"] = 0;
            List<Vocabulary> vocabulary = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == id).ToList();
            if (vocabulary == null)
            {
                return RedirectToAction("Choose_Quiz", "Quiz");
            }
            else
            {
                return View(vocabulary[int.Parse(Session["number"].ToString())]);
            }
        }
        [HttpPost]
        public ActionResult Check_Answer(Vocabulary vocabulary, string answer)
        {
            int number = int.Parse(Session["number"].ToString());
            number++;
            Session["number"] = number;
            Vocabulary vocabulary1 = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == vocabulary.Vocabulary_Code);
            List<Vocabulary> vocabulary2 = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == vocabulary1.Vocabulary_Type_Code).ToList();
            if (vocabulary1 == null)
            {
                return RedirectToAction("Choose_Quiz", "Quiz");
            }
            else
            {
                if (vocabulary1.Vocabulary_English == answer)
                {
                    ViewData["Correct"] = "Correct!";
                }
                Session["Vocabulary"] = vocabulary2[int.Parse(Session["number"].ToString())];
                return RedirectToAction("Continue","Quiz");
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
    }
}