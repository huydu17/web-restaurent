using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models {
	public class ApplicationUser:IdentityUser<Guid> {
        public ApplicationUser() 
        {
            Messages = new List<Message>(); 
        }
		[Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        public DateTime DoB { get;set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Required]
        public string Address { get; set; }
        [NotMapped]
        public string Role { get; set; }
        public List<Conversation> Conversation { get; set; }
        public List<Message> Messages { get; set; }
    }
}
