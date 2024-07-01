using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models {
    public class OrderHeader {
        public int Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public string PaymemtMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentIntentId { get; set; }    
        public string? SessionId { get; set; }  

        [Required(ErrorMessage ="Hãy nhập thông tin")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Hãy nhập thông tin")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Hãy nhập thông tin")]
        public string Address { get; set; }
        public string? OrderNote { get; set; }

    }
}
