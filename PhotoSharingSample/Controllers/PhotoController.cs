using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhotoSharingApplication.Models;   //******************
using System.Drawing;
using System.Security.Permissions;

namespace PhotoSharingApplication.Controllers
{
    public class PhotoController : Controller
    {
        //************************************************************************
        private PhotoSharingDBContext _db = new PhotoSharingDBContext();  // 資料庫的連結

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();   //*************
            base.Dispose(disposing);
        }
        //************************************************************************

        //
        // GET: /Photo/
        // 舊版本，原本的範例首頁
        public ActionResult Index_Old()
        {
            return View("Index_Old", _db.Photos.ToList());
        }

        // 新版本，由我修改過，搭配RWD、Bootstrap
        public ActionResult Index()
        {
            return View("Index", _db.Photos.ToList());
            //return View(db.Photos.ToList());    // 另一種寫法，成果相同。
        }

        //===========================================
        // 舊版本，原本的範例 
        // GET: A Partial View for displaying many photos as cards
        [ChildActionOnly] // 使用者 "不能" 直接透過瀏覽器預覽  // This attribute means the action "cannot" be accessed from the brower's address bar
        public ActionResult _PhotoGallery_Old(int number = 0)
        {
            //We want to display only the latest photos when a positive integer is supplied to the view.
            //Otherwise we'll display them all
            List<Photo> photos;

            if (number == 0)
            {
                photos = _db.Photos.ToList();
            }
            else
            {
                photos = (from p in _db.Photos
                          orderby p.CreatedDate descending
                          select p).Take(number).ToList();   // Take用來「分頁」
            }

            return PartialView("_PhotoGallery_Old", photos);
        }

        // 新版本，由我修改過，搭配RWD、Bootstrap
        // GET: A Partial View for displaying many photos as cards
        [ChildActionOnly] // 使用者不能直接透過瀏覽器預覽  This attribute means the action "cannot" be accessed from the brower's address bar
        public ActionResult _PhotoGallery(int number = 0)
        {
            //We want to display only the latest photos when a positive integer is supplied to the view.
            //Otherwise we'll display them all
            List<Photo> photos;

            if (number == 0)
            {
                photos = _db.Photos.ToList();
            }
            else
            {
                photos = (from p in _db.Photos
                          orderby p.CreatedDate descending
                          select p).Take(number).ToList();    // Take用來「分頁」
            }

            return PartialView("_PhotoGallery", photos);
        }


        //
        // GET: This action shows a jQuery-powered slide show that shows all pics in the application
        public ActionResult SlideShow()
        {
            return View(_db.Photos.ToList());
        }


        //===========================================
        // GET: This actions show the same slideshow view with only the users favorate photos
        public ActionResult FavoritesSlideshow()
        {
            //List<Photo> allPhotos = db.Photos.ToList();
            List<Photo> favPhotos = new List<Photo>();
            List<int> favoriteIds = Session["Favorites"] as List<int>;
            Photo currentPhoto;

            foreach (int favID in favoriteIds)
            {
                currentPhoto = _db.Photos.Find(favID);
                if (currentPhoto != null)
                {
                    favPhotos.Add(currentPhoto);
                }
            }

            return View("SlideShow", favPhotos);
        }

        //===========================================
        // GET: /Photo/5
        // GET: /Photo/Details/5
        public ActionResult Details(int id = 0)
        {
            Photo photo = _db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();  // 找不到
            }
            return View("Details", photo);
        }

        //
        // GET: /Photo/IncompleteAction 
        //This action is to illustrate exception handling
        public ActionResult IncompleteAction()
        {
            throw new NotImplementedException("This Action is not yet complete");
        }


        //===========================================
        // GET: /Photo/Create
        //[Authorize]   // 需要登入，才能使用此功能（新增）
        public ActionResult Create()
        {
            //Create the new photo（新的寫法）
            Photo newPhoto = new Photo
            {    //填入預設值
                UserName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(User.Identity.Name),
                CreatedDate = DateTime.Today,
                ModifiedDate = DateTime.Today
            };

            //// 傳統的寫法
            //Photo newPhoto = new Photo();
            ////填入預設值
            //newPhoto.UserName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(User.Identity.Name);
            //newPhoto.CreatedDate = DateTime.Today;
            //newPhoto.ModifiedDate = DateTime.Today;            

            return View(newPhoto);
        }

        //
        // POST: /Photo/Create
        //[Authorize]   // 需要登入，才能使用此功能（新增）
        [HttpPost]
        public ActionResult Create(Photo photo, HttpPostedFileBase image)
        {   //                                                                    ****************************
            //填入預設值
            photo.UserName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(User.Identity.Name);
            photo.CreatedDate = DateTime.Today;
            photo.ModifiedDate = DateTime.Today;
            if (ModelState.IsValid)
            {
                //Is there a photo? If so save it
                //*** 檔案上傳 ****************************************(start)
                if (image != null)
                {
                    photo.ImageMimeType = image.ContentType;
                    photo.PhotoFile = new byte[image.ContentLength];
                    image.InputStream.Read(photo.PhotoFile, 0, image.ContentLength);
                }
                //*** 檔案上傳 ****************************************(end)

                //Add the photo to the database and save it
                _db.Photos.Add(photo);   // 新增一筆記錄
                _db.SaveChanges();   // 正式寫入資料庫！

                return RedirectToAction("Index");
            }

            return View(photo);
        }



        //===========================================
        // GET: /Photo/Delete/5
        //[Authorize]
        public ActionResult Delete(int id = 0)
        {
            Photo photo = _db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();  // 找不到
            }
            return View(photo);
        }

        //
        // POST: /Photo/Delete/5
        //[Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = _db.Photos.Find(id);   // 先找到這一筆（確定有這筆記錄），再來刪除

            _db.Photos.Remove(photo);   // 刪除一筆記錄
            _db.SaveChanges();   // 正式寫入資料庫！
            return RedirectToAction("Index");
        }


        //===========================================
        // GET: /Photo/Edit/5  （原本範例沒有，這是我們額外增添的功能）
        //[Authorize]
        public ActionResult Edit(int id = 0)   //這個範例有問題，僅供比對。請用下面的 Edit_NEW動作（正確版）
        {  
            Photo photo = _db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();  // 找不到
            }
            return View(photo);
        }

        public ActionResult Edit_NEW(int id = 0)
        {
            Photo photo = _db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();  // 找不到
            }
            return View(photo);
        }


        //
        // POST: /Photo/Edit/5 （原本範例沒有，這是額外增添的功能）
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]   // 避免CSRF攻擊  
        public ActionResult Edit(Photo _photo, HttpPostedFileBase image)
        {   //                                                                 ****************************
            if (ModelState.IsValid)   // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {    
                //*** 檔案上傳 ****************************************(start)
                if (image != null)
                {
                    _photo.ImageMimeType = image.ContentType;

                    _photo.PhotoFile = new byte[image.ContentLength];
                    image.InputStream.Read(_photo.PhotoFile, 0, image.ContentLength);
                }
                //*** 檔案上傳 ****************************************(end)

                // 第一種寫法：
                _db.Entry(_photo).State = System.Data.Entity.EntityState.Modified;  //確認被修改（狀態：Modified）
                _db.SaveChanges();

                //// 第二種寫法：========================================= (start)
                #region
                //// 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
                //Photo ut = _db.Photos.Find(_photo.PhotoID);

                //if (ut == null)
                //{   // 找不到這一筆記錄
                //    return HttpNotFound();
                //}
                //else
                //{
                //    ut.UserName = _photo.UserName;  // 修改後的值

                //    ut.ImageMimeType = _photo.ImageMimeType;
                //    ut.PhotoFile = _photo.PhotoFile;

                //    ut.Description = _photo.Description;
                //    ut.CreatedDate = _photo.CreatedDate;
                //    ut.ModifiedDate = _photo.ModifiedDate;

                //    _db.SaveChanges();   // 回寫資料庫（進行修改）
                //}
                //// 第二種寫法：========================================= (end)
                #endregion

                //return Content(" 更新一筆記錄，成功！");    // 更新成功後，出現訊息（字串）。
                return RedirectToAction("Index");
            }
            else
            {
                return Content(" *** 更新失敗！！*** "); 
            }
            
        }


        //===========================================
        // GET: /Photo/Search
        //[Authorize]   // 需要登入，才能使用此功能（搜尋 - Title 標題）
        public ActionResult Search()
        {   // 做一個空白表單，讓使用者可以輸入「關鍵字」進行搜尋
            return View();
        }

        [HttpPost, ActionName("Search")]
        [ValidateAntiForgeryToken]   // 避免CSRF攻擊
        //public ActionResult Search(string _SearchWord = "MVC")
        public ActionResult SearchList(Photo _photo)
        {
            //第二種寫法：  
            IQueryable<Photo> ListAll = from _Photo in _db.Photos
                                                                   select _Photo;

            //if (!String.IsNullOrEmpty(_SearchWord) && ModelState.IsValid)
            if (!String.IsNullOrEmpty(_photo.Title) && ModelState.IsValid)
            {
                //  把搜尋的結果，傳遞到另一個 SearchList檢視畫面
                // .Contains()對應T-SQL指令的 LIKE，但搜尋關鍵字有「大小寫」的區分
                //                      ************ 重 點！！
                return View("SearchList", ListAll.Where(s => s.Title.Contains(_photo.Title)));
                //                      ************ 重 點！！
            }
            else
            {   // 找不到任何記錄（請參閱最下方的 override HandleUnknowAction()）
                return HttpNotFound();
            }
        }


        //===========================================
        // 另一種搜尋的寫法。   關鍵字的輸入，寫在 Index畫面最下方，必須自己動手寫HTML表單。
        // 透過 HttpGET傳遞關鍵字，例如 SearchList?_SearchWord=MVC  

        // 這裡不寫 [HttpPOST]
        [HttpGet]
        public ActionResult SearchList2(string _SearchWord)
        {
            IQueryable<Photo> ListAll = from _Photo in _db.Photos
                                                                   select _Photo;

            if (!String.IsNullOrEmpty(_SearchWord) && ModelState.IsValid)
            {
                //  把搜尋的結果，傳遞到另一個 SearchList檢視畫面
                //                      ************ 重 點！！
                return View("SearchList", ListAll.Where(s => s.Title.Contains(_SearchWord)));
                //                      ************ 重 點！！
            }
            else
            {   // 找不到任何記錄（請參閱最下方的 override HandleUnknowAction()）
                return HttpNotFound();
            }
        }



        //===========================================
        //== 分頁 ==  LINQ的 .Skip() 與 .Take()。    檢視畫面可套用「 List範本」
        //
        // 可以直接輸入頁數，例如 /Photo/IndexPage/2 或是 /Photo/IndexPage?id=3
        public ActionResult IndexPage(int id = 1)
        {
            // id變數，目前位於第幾頁？
            // PageSize變數，每一頁，要展示幾筆記錄？            
            int PageSize = 1;

            // NowPageCount，目前正在觀賞這一頁的紀錄
            int NowPageCount = 0;
            if (id > 0)   {
                NowPageCount = (id - 1) * PageSize;    // PageSize，每頁展示1筆紀錄（上面設定過了）
            }

            // 這段指令的 .Skip()與 . Take()，其實跟T-SQL指令的 offset...fetch....很類似（SQL 2012起可用）
            var ListAll = (from _photo in _db.Photos
                                    orderby _photo.PhotoID   // 若寫 descending ，則是反排序（由大到小）
                                    select _photo).Skip(NowPageCount).Take(PageSize);    
                                   // .Skip() 從哪裡開始（忽略前面幾筆記錄）。 .Take()呈現幾筆記錄

            //*** 查詢結果 ListAll 是一個 IQueryable ***
            if (ListAll == null)
            {   // 找不到任何記錄
                return HttpNotFound();
            }
            else   {
                return View(ListAll.ToList());
            }
        }



        //===========================================
        //*** 把資料表裡面的「二進位」內容，還原成圖片檔 ****************
        //This action gets the photo file for a given Photo ID
        public FileContentResult GetImage(int PhotoId)
        {
            //Get the right photo
            Photo requestedPhoto = _db.Photos.FirstOrDefault(p => p.PhotoID == PhotoId);
            if (requestedPhoto != null)
            {   //                                                                                    ****************** MIME Type
                return File(requestedPhoto.PhotoFile, requestedPhoto.ImageMimeType);
            }
            else
            {
                return null;
            }
        }


        //===========================================
        //This action adds a photo ID to the list of favorites for the current session
        public ContentResult AddFavorite(int PhotoID)
        {
            List<int> favorites = Session["Favorites"] as List<int>;
            if (favorites == null)
            {
                favorites = new List<int>();
            }
            favorites.Add(PhotoID);

            Session["Favorites"] = favorites;
            return Content("The picture has been added to your favorites（加入 我的最愛）", "text/plain", System.Text.Encoding.Default);
        }

    }
}