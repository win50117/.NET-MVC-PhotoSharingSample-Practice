using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoSharingApplication.Models
{
    public class Photo
    {
        //PhotoID. This is the primary key
        public int PhotoID { get; set; }

        //Title. The title of the photo
        [Required]
        public string Title { get; set; }

        //*******************************************************************
        //PhotoFile. This is a picture file   // 二進位，圖片的二進位內容。
        [DisplayName("Picture")]
        [MaxLength]
        public byte[] PhotoFile { get; set; }
        //*******************************************************************

        //ImageMimeType, stores the MIME type for the PhotoFile
        [HiddenInput(DisplayValue = false)]
        public string ImageMimeType { get; set; }

        //Description.   多列文字
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        //CreatedDate
        [DataType(DataType.DateTime)]
        [DisplayName("Created Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)] // 編輯模式時，也會把「字串格式」帶入
        public DateTime CreatedDate { get; set; }

        //UserName. This is the name of the user who created the photo
        public string UserName { get; set; }

        //ModifiedDate
        [DataType(DataType.DateTime)]
        [DisplayName("Modified Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime ModifiedDate { get; set; }


        //******************************************************************************
        // 一對多的關連式資料表（一筆Photo記錄 對應 多筆Comment）
        //All the comments on this photo, as a navigation property
        public virtual ICollection<Comment> Comments { get; set; }
        //                  *************（重點）一對多
    }
}