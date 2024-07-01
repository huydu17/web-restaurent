using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models
{
    public class Comment 
    {
        public int Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever] 
        
        public ApplicationUser User { get; set; }
        public int Rating { get;set; }
        public string Name { get; set; }
        public string Text { get;set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        
        public Product Product { get; set; }
    }
}
