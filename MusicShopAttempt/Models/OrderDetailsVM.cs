using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShopAttempt.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Models
{
    public class OrderDetailsVM
    {
        [Key]
        public int Id { get; set; }
         

        [Required(ErrorMessage = "This is mandatory!")]
        public int Quantity { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        public double Price { get; set; }


        public int ProductId { get; set; }
        public List<SelectListItem> Product { get; set; }


        public ICollection<SelectListItem> Order { get; set; }
    }
}
