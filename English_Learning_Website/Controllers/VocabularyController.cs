﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using English_Learning_Website.Models;
using PagedList;
using PagedList.Mvc;

namespace English_Learning_Website.Controllers
{
    public class VocabularyController : Controller
    {
        // GET: Vocabulary
        English_Learning_WebsiteEntities1 db = new English_Learning_WebsiteEntities1();
        //List Vocabulary
        public ActionResult ListVocabularyType()
        {
            List<Vocabulary_Type> vocabulary_Type = db.Vocabulary_Type.Where(s => s.Vocabulary_Type_Code != 6).ToList();
            Vocabulary_TypeBonus vocabular_TypeBonus = new Vocabulary_TypeBonus();
            vocabular_TypeBonus.vocabulary_Types = vocabulary_Type;
            return View(vocabular_TypeBonus);
        }
        public ActionResult DeleteVT(int id)
        {
            try
            {
                Vocabulary_Type vocabulary_Type = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_Code == id);
                if(vocabulary_Type != null)
                {
                    db.Vocabulary_Type.Remove(vocabulary_Type);
                    db.SaveChanges();
                    List<Vocabulary_Type> vocabulary_Type1 = db.Vocabulary_Type.ToList();
                }
                return RedirectToAction("ListVocabularyType");
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult EditVT(int id)
        {
            Vocabulary_Type vocabulary_Type = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_Code == id);
            Vocabulary_TypeBonus vocabular_TypeBonus = new Vocabulary_TypeBonus();
            vocabular_TypeBonus.Vocabulary_Type_Code = vocabulary_Type.Vocabulary_Type_Code;
            vocabular_TypeBonus.Vocabulary_Type_EN_Name = vocabulary_Type.Vocabulary_Type_EN_Name;
            return View(vocabular_TypeBonus);
        }
        [HttpPost]
        public ActionResult EditVT(Vocabulary_TypeBonus vocabulary_TypeBonus)
        {
            Vocabulary_Type vocabulary_Type2 = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_EN_Name == vocabulary_TypeBonus.Vocabulary_Type_EN_Name);
            Vocabulary_Type vocabulary_Type1 = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_Code == vocabulary_TypeBonus.Vocabulary_Type_Code);
            try
            {
                if (vocabulary_Type2 == null)
                {
                    vocabulary_Type1.Vocabulary_Type_EN_Name = vocabulary_TypeBonus.Vocabulary_Type_EN_Name;
                    db.Vocabulary_Type.AddOrUpdate(vocabulary_Type1);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabularyType", "Vocabulary");
                }
                else
                {
                    ViewData["existed"] = "Vocabulary type name existed";
                    return View(vocabulary_TypeBonus);
                }
            }
           catch
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        public ActionResult CreateVT(Vocabulary_TypeBonus vocabular_TypeBonus)
        {
            Vocabulary_Type vocabulary_Type2 = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_EN_Name == vocabular_TypeBonus.Vocabulary_Type_EN_Name);
            try
            {
                if (vocabular_TypeBonus != null)
                {
                    if (vocabulary_Type2 == null)
                    {
                        Vocabulary_Type vocabulary_Type1 = new Vocabulary_Type();
                        vocabulary_Type1.Vocabulary_Type_EN_Name = vocabular_TypeBonus.Vocabulary_Type_EN_Name;
                        db.Vocabulary_Type.Add(vocabulary_Type1);
                        db.SaveChanges();
                        return RedirectToAction("ListVocabularyType", "Vocabulary");
                    }
                    else
                    {
                        return View(vocabular_TypeBonus);
                    }
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult ListVocabulary(int? page, string searchVocabulary1, string sortVocabulary1)
        {
            List<Vocabulary> vocabularies = new List<Vocabulary>();
            if (searchVocabulary1 != null)
            {
                Session["searchVocabulary1"] = searchVocabulary1;
                vocabularies = db.Vocabularies.Where(s => s.Vocabulary_English.Contains(searchVocabulary1.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchVocabulary1"] = null;
                vocabularies = db.Vocabularies.ToList();
            }
            List<Vocabulary> vocabularies1;
            if (sortVocabulary1 == null || sortVocabulary1 == "None")
            {
                Session["Vocabulary_Sort1"] = "None";
                vocabularies1 = vocabularies;
            }
            else if (sortVocabulary1 == "AZ")
            {
                Session["Vocabulary_Sort1"] = "A - Z";
                vocabularies1 = vocabularies.OrderBy(s => s.Vocabulary_English).ToList();
            }
            else
            {
                Session["Vocabulary_Sort1"] = "Z - A";
                vocabularies1 = vocabularies.OrderByDescending(s => s.Vocabulary_English).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(vocabularies1.ToPagedList(pageNum, pageSize));
        }
        public ActionResult DetailV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
            return View(vocabulary);
        }
        public async Task<ActionResult> Speech(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
            Task<RedirectToRouteResult> viewResult = Task.Run(() =>
             {
                 using (SpeechSynthesizer sp = new SpeechSynthesizer())
                 {
                     sp.SelectVoice("Microsoft Zira Desktop");
                     sp.SetOutputToDefaultAudioDevice();
                     sp.Speak(vocabulary.Vocabulary_English);
                     return RedirectToAction("DetailV/" + id);
                 }
             });
            return await viewResult;
        }
        public ActionResult DeleteV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
            return View(vocabulary);
        }
        public ActionResult DeleteVC(int id)
        {
            try
            {
                Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
                if(vocabulary != null)
                {
                    db.Vocabularies.Remove(vocabulary);
                    db.SaveChanges();
                }
                return RedirectToAction("ListVocabulary");
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult CreateV()
        {
            List<Vocabulary_Type> vocabulary_Types = db.Vocabulary_Type.ToList();
            VocabularyBonus vocabularyBonus = new VocabularyBonus();
            vocabularyBonus.vocabulary_Types = vocabulary_Types;
            return View(vocabularyBonus);
        }
        [HttpPost]
        public ActionResult CreateV(VocabularyBonus vocabularyBonus)
        {
            try
            {
                if(vocabularyBonus != null)
                {
                    Vocabulary vocabulary = new Vocabulary();
                    vocabulary.Vocabulary_Type_Code = vocabularyBonus.Vocabulary_Type_Code;
                    vocabulary.Vocabulary_English = vocabularyBonus.Vocabulary_English;
                    vocabulary.Vocabulary_Vietnamese = vocabularyBonus.Vocabulary_Vietnamese;
                    vocabulary.Vocabulary_Pronunciation = vocabularyBonus.Vocabulary_Pronunciation;
                    if (vocabularyBonus.Vocabulary_Image != null)
                    {
                        vocabulary.Vocabulary_Image = vocabularyBonus.Vocabulary_Image;
                    }
                    else
                    {
                        vocabulary.Vocabulary_Image = "1478594.png";
                    }
                    db.Vocabularies.Add(vocabulary);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabulary", "Vocabulary");
                }
                return this.CreateV();
            }
            catch
            {
                return HttpNotFound();
            }
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
        public ActionResult EditV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
            if(vocabulary != null)
            {
                VocabularyBonus vocabularyBonus = new VocabularyBonus();
                vocabularyBonus.Vocabulary_Code = vocabulary.Vocabulary_Code;
                vocabularyBonus.Vocabulary_Image = vocabulary.Vocabulary_Image;
                vocabularyBonus.Vocabulary_English = vocabulary.Vocabulary_English;
                vocabularyBonus.Vocabulary_Vietnamese = vocabulary.Vocabulary_Vietnamese;
                vocabularyBonus.Vocabulary_Pronunciation = vocabulary.Vocabulary_Pronunciation;
                vocabularyBonus.Vocabulary_Type_Code = vocabulary.Vocabulary_Type.Vocabulary_Type_Code;
                List<Vocabulary_Type> vocabulary_Types = db.Vocabulary_Type.ToList();
                vocabularyBonus.vocabulary_Types = vocabulary_Types;
                return View(vocabularyBonus);
            }
            else
            {
                return RedirectToAction("ListVocabulary", "Vocabulary");
            }
        }
        [HttpPost]
        public ActionResult EditV(VocabularyBonus vocabularyBonus)
        {
            try
            {
                if (vocabularyBonus != null)
                {
                    Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == vocabularyBonus.Vocabulary_Code);
                    vocabulary.Vocabulary_Type_Code = vocabularyBonus.Vocabulary_Type_Code;
                    vocabulary.Vocabulary_English = vocabularyBonus.Vocabulary_English;
                    vocabulary.Vocabulary_Vietnamese = vocabularyBonus.Vocabulary_Vietnamese;
                    vocabulary.Vocabulary_Pronunciation = vocabularyBonus.Vocabulary_Pronunciation;
                    vocabulary.Vocabulary_Image = vocabularyBonus.Vocabulary_Image;
                    db.Vocabularies.AddOrUpdate(vocabulary);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabulary", "Vocabulary");
                }
                return View(vocabularyBonus);
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult User_View(int? id, int? page, string searchVocabulary, string sortVocabulary)
        {
            List<Vocabulary_Type> vocabulary_Types = db.Vocabulary_Type.Where(s => s.Vocabulary_Type_Code != 6).ToList();
            Session["Index"] = vocabulary_Types;
            List<Vocabulary> vocabularies = new List<Vocabulary>();
            if (searchVocabulary != null)
            {
                Session["searchVocabulary"] = searchVocabulary;
                vocabularies = db.Vocabularies.Where(s => s.Vocabulary_English.Contains(searchVocabulary.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchVocabulary"] = null;
                if (id == null)
                {
                    vocabularies = db.Vocabularies.ToList();
                }
                if (id != null)
                {
                    Vocabulary_Type vocabulary_Type = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_Code == id);
                    vocabularies = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == vocabulary_Type.Vocabulary_Type_Code).ToList();
                }
            }
            List<Vocabulary> vocabularies1;
            if (sortVocabulary == null || sortVocabulary == "None")
            {
                Session["Vocabulary_Sort"] = "None";
                vocabularies1 = vocabularies;
            }
            else if (sortVocabulary == "AZ")
            {
                Session["Vocabulary_Sort"] = "A - Z";
                vocabularies1 = vocabularies.OrderBy(s => s.Vocabulary_English).ToList();
            }
            else
            {
                Session["Vocabulary_Sort"] = "Z - A";
                vocabularies1 = vocabularies.OrderByDescending(s => s.Vocabulary_English).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(vocabularies1.ToPagedList(pageNum, pageSize));
        }
    }
}