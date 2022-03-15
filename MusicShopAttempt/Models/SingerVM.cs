using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShopAttempt.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Models
{
    public class SingerVM
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [MaxLength(50)]
        public string SingerName { get; set; }

        public List<SelectListItem> Product { get; set; }
    }
}
