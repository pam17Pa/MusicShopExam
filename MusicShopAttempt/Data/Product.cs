using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }

        [NotMapped]
        public IFormFile PictureFile { get; set; }
        public double Price { get; set; }

        public DateTime EntryDate { get; set; }
        public StatusType Status { get; set; }
        public PromoType Promo { get; set; }
        public HolderType Holder { get; set; }
        public CategoryType Category { get; set; }

        public int SingerId { get; set; }
        public Singer Singer { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public int Quantity { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
