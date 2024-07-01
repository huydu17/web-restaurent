using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class ChatViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public IEnumerable<Message> Messages { get; set; } 
    }
}
