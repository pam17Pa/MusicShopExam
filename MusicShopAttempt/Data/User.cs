using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Data
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public ICollection<Order> Order { get; set; }
    }
}
