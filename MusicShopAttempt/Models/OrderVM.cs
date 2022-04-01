using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShopAttempt.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Models
{
    public class OrderVM
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of order")]
        public DateTime OrderedOn { get; set; }


        public string UserId { get; set; }
        public  User User { get; set; }


        public int OrderDetailsId { get; set; }
        public List<SelectListItem> OrderDetails { get; set; }
    }
}
