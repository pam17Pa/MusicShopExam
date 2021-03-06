using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShopAttempt.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Models
{
    public class ProductVM
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public IFormFile PictureFile { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        public double Price { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of order")]
        public DateTime EntryDate { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [EnumDataType(typeof(StatusType))]
        public StatusType Status { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [EnumDataType(typeof(PromoType))]
        public PromoType Promo { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [EnumDataType(typeof(HolderType))]
        public HolderType Holder { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [EnumDataType(typeof(CategoryType))]
        public CategoryType Category { get; set; }


        public int SingerId { get; set; }
        public Singer SingerNow { get; set; }
        public List<SelectListItem> Singer { get; set; }


        public int GenreId { get; set; }
        public Genre GenreNow { get; set; }
        public List<SelectListItem> Genre { get; set; }

        public int Quantity { get; set; }
        public string UserId { get; set; }


        //public ICollection<Order> Order { get; set; }
        //public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
