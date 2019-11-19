using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PhotoSharingApplication.Models
{
    //*******************************
    // 資料內容類別 DbContext   -- 用來記錄您的資料庫有下面這兩個「資料表」
    //*******************************
    public partial class PhotoSharingDBContext : DbContext
    {
        public PhotoSharingDBContext()
                  : base("name=PhotoSharingDB")   // DB連結字串，務必與「資料庫名稱」相同！！
        {
        }

        public DbSet<Photo> Photos { get; set; }  // 注意！後面是複數（s）
        public DbSet<Comment> Comments { get; set; }  // 注意！後面是複數（s）
    }
}