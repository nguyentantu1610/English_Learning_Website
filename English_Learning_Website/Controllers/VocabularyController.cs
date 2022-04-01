using System;
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
        English_Learning_WebsiteEntities2 db = new English_Learning_WebsiteEntities2();

        //public ActionResult Index(string sortOrder)
        //{
        //    ViewBag.CodeSortParm = String.IsNullOrEmpty(sortOrder) ? "code_decs" : "";
        //    ViewBag.TenTVSortParm = sortOrder == "tentv" ? "tentv_decs" : "Date";
        //    var voca = db.Vocabularies.AsQueryable();
        //    switch (sortOrder)
        //    {
        //        case "code_decs": 
        //            voca = voca.OrderByDescending(s => s.Vocabulary_Code);
        //            break;
        //        case "tentv":
        //            voca = voca.OrderBy(s => s.Vocabulary_English);
        //            break;
        //        case "tentv_decs":
        //            voca = voca.OrderByDescending(s => s.Vocabulary_English);
        //            break;
        //        default:
        //            voca = voca.OrderBy(s => s.Vocabulary_Code);
        //            break;
        //    }
        //    return View(voca.ToList());
        //}
        public ActionResult Index(string sortOrder)
        {
            // 1. Thêm biến NameSortParm để biết trạng thái sắp xếp tăng, giảm ở View
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            // 2. Truy vấn lấy tất cả đường dẫn
            var voca = from l in db.Vocabularies
                        select l;
            // 3. Thứ tự sắp xếp theo thuộc tính LinkName
            switch (sortOrder)
            {
                // 3.1 Nếu biến sortOrder sắp giảm thì sắp giảm theo LinkName
                case "name_desc":
                    voca = voca.OrderByDescending(s => s.Vocabulary_English);
                    break;

                // 3.2 Mặc định thì sẽ sắp tăng
                default:
                    voca = voca.OrderBy(s => s.Vocabulary_English);
                    break;
            }

            // 4. Trả kết quả về Views
            return View(voca.ToList());
        }
            //List Vocabulary
        public ActionResult ListVocabularyType()
        {
            List<Vocabulary_Type> vocabulary_Type = db.Vocabulary_Type.ToList();
            Vocabulary_TypeBonus vocabular_TypeBonus = new Vocabulary_TypeBonus();
            vocabular_TypeBonus.vocabulary_Types = vocabulary_Type;
            return View(vocabular_TypeBonus);
        }
        public ActionResult DeleteVT(int id)
        {
            Vocabulary_Type vocabulary_Type = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_Code == id);
            db.Vocabulary_Type.Remove(vocabulary_Type);
            db.SaveChanges();
            List<Vocabulary_Type> vocabulary_Type1 = db.Vocabulary_Type.ToList();
            return RedirectToAction("ListVocabularyType");
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
        public ActionResult ListVocabulary(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 3;
            int pageNum = page ?? 1;
            List<Vocabulary> vocabularies = db.Vocabularies.ToList();
            return View(vocabularies.ToPagedList(pageNum, pageSize));
        }
        public ActionResult DetailV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
            return View(vocabulary);
        }
        public async Task<ActionResult> Speech(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
            Task<ViewResult> viewResult = Task.Run(() =>
             {
                 using (SpeechSynthesizer sp = new SpeechSynthesizer())
                 {
                     sp.SelectVoice("Microsoft Zira Desktop");
                     sp.SetOutputToDefaultAudioDevice();
                     sp.Speak(vocabulary.Vocabulary_English);
                     return View(vocabulary);
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
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.Vocabulary_Code == id);
            db.Vocabularies.Remove(vocabulary);
            db.SaveChanges();
            return RedirectToAction("ListVocabulary");
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
                    //vocabulary.Vocabulary_Description = vocabularyBonus.Vocabulary_Description;
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
                return View(vocabularyBonus);
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
                //vocabularyBonus.Vocabulary_Description = vocabulary.Vocabulary_Description;
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
                    //vocabulary.Vocabulary_Description = vocabularyBonus.Vocabulary_Description;
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
        public ActionResult User_View(int? id, int? page)
        {
            if (page == null) page = 1;
            int pageSize = 9;
            int pageNum = page ?? 1;
            if (id == null)
            {
                List<Vocabulary> vocabularies = db.Vocabularies.ToList();
                List<Vocabulary_Type> vocabulary_Types = db.Vocabulary_Type.ToList();
                Session["Index"] = vocabulary_Types;
                return View(vocabularies.ToPagedList(pageNum, pageSize));
            }
            else
            {
                Vocabulary_Type vocabulary_Type = db.Vocabulary_Type.FirstOrDefault(s => s.Vocabulary_Type_Code == id);
                List<Vocabulary> vocabularies = db.Vocabularies.Where(s => s.Vocabulary_Type_Code == vocabulary_Type.Vocabulary_Type_Code).ToList();
                return View(vocabularies.ToPagedList(pageNum, pageSize));
            }
        }
        
    }
}