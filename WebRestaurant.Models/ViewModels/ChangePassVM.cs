using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class ChangePassVM
    {    
        public string CurrentPass { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Độ dài tối thiểu 6 kí tự và phải chứa 1 chữ hoa, 1 chữ thường, 1 ký tự đặc biệt và 1 chữ số")]
        public string NewPass { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Compare("NewPass", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ComfirmPass { get; set; }
    }
}
