using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models
{
    public class Contact
    {
        [Required(ErrorMessage ="Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng email")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tiêu đề")]
        public string Subject { set; get; }
        [Required(ErrorMessage = "Vui lòng nội dung")]
        public string Message { get; set; }
    }
}
