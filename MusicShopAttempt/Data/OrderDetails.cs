using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Data
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public ICollection<Order> Order { get; set; }

      
    }
}
