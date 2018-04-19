using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.IO;
using System.Data.SqlTypes;

namespace WebApplication1.Controllers
{
    public class newsController : Controller
    {
        private NSHNContext db = new NSHNContext();

        // GET: news
        public ActionResult Index()
        {
            return View(db.news.ToList());
        }

        public PartialViewResult MainImgs(int id)
        {
            image img = new image();
            //SELECT ALL IMAGES FOR THIS ARTICLE ID
            List<image> imageList = db.images.Where(i => i.news_article_id == id).ToList();

            //SELECT ALL IMAGES FOR THIS ARTICLE WHERE THE IMAGE IS MAIN
            imageList = imageList.Where(i => i.is_main == 1).ToList();

            return PartialView("~/Views/news_partials/_MainImgs.cshtml", imageList);
        }

        // GET: news/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        public PartialViewResult ImagesAllForArticle(int id)
        {
            //GET THE LIST OF ALL IMAGES FROM THE ARTICLE ID
            List<image> images = db.images.Where(i => i.news_article_id == id).ToList();
            return PartialView("~/Views/news_partials/_ImagesAllForArticle.cshtml", images);
        }
        [HttpPost]
        public void SaveCaption(int id)
        {
            RedirectToAction("Details", id);
        }

        /*********************************************************/
        [HttpGet]
        public PartialViewResult DisplayTestAjax(int id)
        {
            //GET THE LIST OF ALL IMAGES FROM THE ARTICLE ID
            List<image> images = db.images.Where(i => i.news_article_id == id).ToList();
            return PartialView("~/Views/news_partials/_DisplayPhotoCaptionCreate.cshtml", images);
        }

        [HttpPost]
        public void SaveImgCaption(int id, FormCollection form)
        {
            if(ModelState.IsValid)
            {
                //SAVE THE CAPTION FOR THE CORRECT IMAGE
                string caption = form["item.caption"].ToString();
                int imgId = Convert.ToInt16(form["item.id"]);

                //GET THE IMAGE PROPERTIES FROM THE IMG ID 
                image articleImage = db.images.Find(imgId);

                //MODIFY THE IMAGE CAPTION
                articleImage.caption = caption;

                db.Entry(articleImage).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        /*********************************************************/

        //REDIRECTED HERE FROM CREATE TO THEN ADD A CAPTION
        [HttpGet]
        public ActionResult EditCaption(int? id)
        {
            news news = db.news.Find(id);
            return View(news);
        }

        // GET: news/Create
        public ActionResult Create()
        {
            return View();
        }

        //ALLOW USER TO CREATE A FILE
        public PartialViewResult DisplayFileUpload()
        {
            return PartialView("~/Views/news_partials/_images.cshtml");
        }

        // POST: news/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,pub_date,article_content,article_summary,author")] news news, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                //ADD TO NEWS
                news.pub_date = DateTime.Now;
                news.title.Trim();
                news.article_summary.Trim();
                news.article_content.Trim();
                db.news.Add(news);
                db.SaveChanges();

                //ADD TO IMAGES
                image img = new image();
                List<string> filenames = new List<string>();

                if (files.Count() > 0)
                {
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            //ADD EACH FILE NAME INTO AN ARRAY
                            string filename = Path.GetFileName(file.FileName);
                            filenames.Add(filename);

                            //SET THE IMG SOURCE TO THE FILE NAME
                            img.img_src = filename;

                            //SET THE MAIN IMAGE ID
                            if (filenames.Count() == 1)
                            {
                                img.is_main = 1;
                            }
                            else
                            {
                                img.is_main = 0;
                            }

                            //SET THE NEWS ARTICLE ID TO THE CURRENT ONE
                            img.news_article_id  = news.id;
                            db.images.Add(img);

                            //ADD THE IMAGES TO YOUR SERVER
                            //string path = Path.Combine(Server.MapPath("~/News_Images/" + img.id.ToString() + "/"));
                            string path = Path.Combine(Server.MapPath("~/News_Images/" + news.id.ToString() + "/"));
                            Directory.CreateDirectory(path);
                            path = Path.Combine(Server.MapPath("~/News_Images/" + news.id.ToString() + "/"), filename);

                            file.SaveAs(path);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("EditCaption", new { news.id });
                }
                else
                {
                    ViewBag.ErrorMssg = "The file you've uploaded is corrupt";
                    return View();
                }
            }
             var errors = ModelState.Select(x => x.Value.Errors)
            .Where(y => y.Count > 0)
            .ToList();
            return View(news);
        }
        
        // GET: news/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        public PartialViewResult EditPhoto(int? id)
        {
            //FIND IMAGES ASSOCIATED WITH THIS ARTICLE ID
            return PartialView("~/Views/news_partials/_EditPhoto.cshtml", db.images.Where(i => i.news_article_id == id).ToList());
        }

        public ActionResult DeletePhoto(int? id)
        {
            image img = db.images.Find(id);
            int articleId = img.news_article_id;
            string img_src = img.img_src;

            //IF THE IMAGE SELECTED IS EQUAL TO MAIN, AND IF THERE ARE MANY PHOTOS, ASSIGN THE MAIN TO ANOTHER PHOTO
            List<image> images = db.images.Where(i => i.news_article_id == articleId).ToList();
            if(img.is_main == 1 && images.Count() > 1)
            {
                images[1].is_main = 1;
                db.SaveChanges();
            }

            //REMOVE THE IMG SOURCE
            db.images.Remove(img);
            db.SaveChanges();

            //REMOVE FROM DIRECTORY
            string pathToDirectory = Request.MapPath("~/News_Images/" + articleId.ToString());
            DirectoryInfo DirInfo = new DirectoryInfo(pathToDirectory);

            foreach(FileInfo file in DirInfo.GetFiles())
            {
                if(file.Name == img_src)
                {
                    file.Delete();
                }
            }

            //IF THERE AREN'T ANY MORE FILES IN THE DIRECTORY, DELETE THE DIRECTORY
            if(!Directory.EnumerateFileSystemEntries(pathToDirectory).Any())
            {
                Directory.Delete(pathToDirectory);
            }

            return RedirectToAction("Edit", "News", new { id = articleId });

        }

        // POST: news/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,author,pub_date,article_content,article_summary")] news news, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                //UPDATE NEWS
                news.title.Trim();
                news.article_content.Trim();
                news.article_summary.Trim();
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");

                //ADDING NEW PHOTOS
                image img = new image();

                //LIST ALL THE IMAGES FOR THIS ARTICLE
                List<image> imageList = db.images.Where(i => i.news_article_id == news.id).ToList();

                //DETERMINE IF THE ANY OF THE IMAGES FOR THIS ARTICLE HAVE A MAIN ID
                var main_images = imageList.Where(i => i.is_main == 1);
                int count = 0;
                foreach (var main in main_images)
                {
                    count++;
                }

                if (files.Count() > 0)
                {
                    foreach (HttpPostedFileBase file in files)
                    {
                        if (file != null)
                        {
                            //GET THE FILENAME OF EACH FILE
                            string filename = Path.GetFileName(file.FileName);

                            //SET THE IMG SOURCE TO THE FILE NAME
                            img.img_src = filename;

                            //SET THE MAIN IMAGE ID
                            if (count == 0)
                            {
                                img.is_main = 1;
                            }
                            else
                            {
                                img.is_main = 0;
                            }

                            //SET THE NEWS ARTICLE ID TO THE CURRENT ONE
                            img.news_article_id = news.id;
                            db.images.Add(img);

                            //ADD THE IMAGES TO YOUR SERVER
                            //string path = Path.Combine(Server.MapPath("~/News_Images/" + img.id.ToString() + "/"));
                            string path = Path.Combine(Server.MapPath("~/News_Images/" + news.id.ToString() + "/"));
                            Directory.CreateDirectory(path);
                            path = Path.Combine(Server.MapPath("~/News_Images/" + news.id.ToString() + "/"), filename);

                            file.SaveAs(path);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Edit");
                }
                return RedirectToAction("Edit");
            }
            return RedirectToAction("Edit");
        }

        // GET: news/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: news/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //DELETE IMAGES FROM DATABASE
            List<image> images = db.images.Where(i => i.news_article_id == id).ToList();

            foreach(image img in images)
            {
                db.images.Remove(img);
            }

            //DELETE FROM NEWS
            news news = db.news.Find(id);
            db.news.Remove(news);
            db.SaveChanges();

            //REMOVE FROM DIRECTORY
            string pathToDirectory = Request.MapPath("~/News_Images/" + id.ToString());
            DirectoryInfo DirInfo = new DirectoryInfo(pathToDirectory);

            foreach (FileInfo file in DirInfo.GetFiles())
            {
                file.Delete();
            }

            //IF THERE AREN'T ANY MORE FILES IN THE DIRECTORY, DELETE THE DIRECTORY
            if (!Directory.EnumerateFileSystemEntries(pathToDirectory).Any())
            {
                Directory.Delete(pathToDirectory);
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}


//USEFUL LINKS
/*
 * UNDERSTANDING EDITS AND BIND VARIABLES
 * https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/examining-the-edit-methods-and-edit-view
*/

/*
         public ActionResult AddNewPhoto(int id, HttpPostedFileBase[] files)
    {
        //ADD TO IMAGES
        image img = new image();

        //LIST ALL THE IMAGES FOR THIS ARTICLE
        List<image> imageList = db.images.Where(i => i.news_article_id == id).ToList();

        //DETERMINE IF THE ANY OF THE IMAGES FOR THIS ARTICLE HAVE A MAIN ID
        var main_images = imageList.Where(i => i.is_main == 1);
        int count = 0;
        foreach (var main in main_images)
        {
            count++;
        }

        if (files.Count() > 0)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null)
                {
                    //GET THE FILENAME OF EACH FILE
                    string filename = Path.GetFileName(file.FileName);

                    //SET THE IMG SOURCE TO THE FILE NAME
                    img.img_src = filename;

                    //SET THE MAIN IMAGE ID
                    if (count == 0)
                    {
                        img.is_main = 1;
                    }
                    else
                    {
                        img.is_main = 0;
                    }

                    //SET THE NEWS ARTICLE ID TO THE CURRENT ONE
                    img.news_article_id = id;
                    db.images.Add(img);

                    //ADD THE IMAGES TO YOUR SERVER
                    //string path = Path.Combine(Server.MapPath("~/News_Images/" + img.id.ToString() + "/"));
                    string path = Path.Combine(Server.MapPath("~/News_Images/" + id.ToString() + "/"));
                    Directory.CreateDirectory(path);
                    path = Path.Combine(Server.MapPath("~/News_Images/" + id.ToString() + "/"), filename);

                    file.SaveAs(path);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Edit");
        }
        return RedirectToAction("Edit");
    }
 */
