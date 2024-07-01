using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Tài khoản không hợp lệ")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]

        public string Password { get;set; }
    }
}
