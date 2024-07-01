using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage ="Vui lòng nhập họ")]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string UserName { get; set; }
        public DateTime DoB { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Độ dài tối thiểu 6 kí tự và phải chứa 1 chữ hoa, 1 chữ thường, 1 ký tự đặc biệt và 1 chữ số")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Compare("Password",ErrorMessage ="Mật khẩu không trùng khớp")]
        public string PasswordConfirm { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get;set; }
        public Guid? RoleId { get; set; }    
        [ValidateNever]
        public IEnumerable<SelectListItem> ListRole { get; set; }
    }
}
