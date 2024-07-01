using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class ProductDetailVM
    {
        public ShoppingCart ShoppingCart { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
