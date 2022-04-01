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
    public class StoryController : Controller
    {
        // GET: Story
        English_Learning_WebsiteEntities2 db = new English_Learning_WebsiteEntities2();
        public ActionResult ListStory(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 3;
            int pageNum = page ?? 1;
            List<Story> story = db.Stories.ToList();
            List<StoryBonus> storyBonus = new List<StoryBonus>();
            foreach (Story item in story)
            {
                StoryBonus storyBonus1 = new StoryBonus();
                storyBonus1.Story_Code = item.Story_Code;
                storyBonus1.Story_image = item.Story_image;
                storyBonus1.Story_Name = item.Story_Name;
                Userz userz = db.Userzs.FirstOrDefault(s => s.User_Code == item.Userz_Code);
                storyBonus1.Userz_FullName = userz.User_FullName;
                storyBonus.Add(storyBonus1);
            }
            return View(storyBonus.ToPagedList(pageNum, pageSize));
        }
        public ActionResult CreateS()
        {
            StoryBonus story = new StoryBonus();
            return View(story);
        }
        [HttpPost]
        public ActionResult CreateS(StoryBonus story)
        {
            try
            {
                Story story1 = new Story();
                if (story.Story_banner != null)
                {
                    story1.Story_banner = story.Story_banner;
                }
                else
                {
                    story1.Story_banner = "1478594.png";
                }
                if (story.Story_image != null)
                {
                    story1.Story_image = story.Story_image;
                }
                else
                {
                    story1.Story_image = "1478594.png";
                }
                story1.Story_Name = story.Story_Name;
                story1.Story_Content_EN = story.Story_Content_EN;
                story1.Story_Content_VN = story.Story_Content_VN;
                story1.Story_Audio = story.Story_Audio;
                story1.Userz_Code = story.Userz_Code;
                story1.Story_View = 0;
                db.Stories.AddOrUpdate(story1);
                db.SaveChanges();
                return RedirectToAction("ListStory", "Story");
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult EditS(int id)
        {
            Story story = db.Stories.FirstOrDefault(s => s.Story_Code == id);
            StoryBonus storyBonus = new StoryBonus();
            storyBonus.Story_Code = story.Story_Code;
            storyBonus.Story_banner = story.Story_banner;
            storyBonus.Story_image = story.Story_image;
            storyBonus.Story_Name = story.Story_Name;
            storyBonus.Story_Content_EN = story.Story_Content_EN;
            storyBonus.Story_Content_VN = story.Story_Content_VN;
            storyBonus.Story_Audio = story.Story_Audio;
            storyBonus.Userz_Code = story.Userz.User_Code;
            return View(storyBonus);
        }
        [HttpPost]
        public ActionResult EditS(StoryBonus storyBonus)
        {
            try
            {
                Story story = db.Stories.FirstOrDefault(s => s.Story_Code == storyBonus.Story_Code);
                story.Story_banner = storyBonus.Story_banner;
                story.Story_image = storyBonus.Story_image;
                story.Story_Name = storyBonus.Story_Name;
                story.Story_Content_EN = storyBonus.Story_Content_EN;
                story.Story_Content_VN = storyBonus.Story_Content_VN;
                story.Story_Audio = storyBonus.Story_Audio;
                db.Stories.AddOrUpdate(story);
                db.SaveChanges();
                return RedirectToAction("ListStory", "Story");
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult DetailS(int id)
        {
            Story story = db.Stories.FirstOrDefault(s => s.Story_Code == id);
            if (story != null)
            {
                story.Story_View += 1;
                db.Stories.AddOrUpdate(story);
                db.SaveChanges();
                return View(story);
            }
            return RedirectToAction("ListStory", "Story");
        }
        public ActionResult DeleteS(int id)
        {
            Story story = db.Stories.FirstOrDefault(s => s.Story_Code == id);
            return View(story);
        }
        public ActionResult DeleteSS(int id)
        {
            Story story = db.Stories.FirstOrDefault(s => s.Story_Code == id);
            db.Stories.Remove(story);
            db.SaveChanges();
            return RedirectToAction("ListStory", "Story");
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
        public ActionResult User_View(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 3;
            int pageNum = page ?? 1;
            List<Story> story = db.Stories.ToList();
            return View(story.ToPagedList(pageNum, pageSize));
        }
    }
}