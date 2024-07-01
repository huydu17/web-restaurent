using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models.ViewModels
{
    public class BookTableVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn số người")]
        public int NumberOfPeople { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giờ")]
        public string ArrivalDate { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giờ")]
        public string ArrivalTime { get; set; }
        public string Message { get; set; }
        public bool Status { get;set; }
    }
}
