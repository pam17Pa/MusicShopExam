using Microsoft.AspNetCore.Mvc.Rendering;
using MusicShopAttempt.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Models
{
    public class UserVM
    {
        [Required(ErrorMessage = "This is mandatory!")]
        [MaxLength(50)]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "This is mandatory!")]
        [MaxLength(50)]
        public string Surname { get; set; }

        public ICollection<SelectListItem> Order { get; set; }
    }
}
