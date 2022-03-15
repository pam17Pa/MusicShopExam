using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Data
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderedOn { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int OrderDetailsId { get; set; }
        public OrderDetails OrderDetails { get; set; }
    }
}
