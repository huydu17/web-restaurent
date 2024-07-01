using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models
{
    public class Conversation
    {
        public Conversation() 
        {
            Messages = new List<Message>(); 
        }
        public int Id { get; set; } 
        public string Name { get; set; }  
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser AppUser { get;set; }
        public List<Message> Messages { get; set; }
    }
}
