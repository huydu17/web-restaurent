using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models
{
    public class Message 
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public Guid SenderId { get; set; }
        public ApplicationUser AppSender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
