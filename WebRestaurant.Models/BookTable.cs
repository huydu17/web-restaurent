using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models
{
	public class BookTable
	{
		[Key]
		public int Id { get; set; }
		public Guid UserId { get; set; }
		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }
		[Required(ErrorMessage ="Vui lòng nhập tên")]
		public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; }
		public DateTime CreateDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Vui lòng nhập giờ")]
        public DateTime ArrivalDatetime { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số người")]
        public int NumberOfPeople { get; set; }
		[NotMapped]
		public string ArrivalDate => ArrivalDatetime.ToString("dd/MM/yyyy");
		[NotMapped]
		public string ArrivalTime => ArrivalDatetime.ToString("hh:mm tt");
		public string Message { get; set; }
		public string Status { get; set; }
	}
}
