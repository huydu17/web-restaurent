using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class CartItemVM
    {
        public Guid GuidId { get; set; } = Guid.NewGuid();  
        public Product Product { get; set; }    
        public int Quantity { get; set; }   
    }
}
