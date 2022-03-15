using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Data
{
    public class Genre
    {
        public int Id { get; set; }
        public string GenreName { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
