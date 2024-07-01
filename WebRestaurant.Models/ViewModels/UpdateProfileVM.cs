using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class UpdateProfileVM
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập họ")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }
    }
}
