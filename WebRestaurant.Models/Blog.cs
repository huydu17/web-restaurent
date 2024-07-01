using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models
{
    public class Blog
    {
        public Blog() 
        {
            Status = true;
        }
        public int Id { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tiêu đề")]        
        public string Title { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả ngắn")]
        public string ShortDescription { get;set; }
        [Required(ErrorMessage = "Vui lòng nhập tác giả")]         
        public string Author { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
