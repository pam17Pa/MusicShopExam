using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShopAttempt.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Models
{
    public class GenreVM
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        public string GenreName { get; set; }


        public ICollection<SelectListItem> Product { get; set; }
    }
}
