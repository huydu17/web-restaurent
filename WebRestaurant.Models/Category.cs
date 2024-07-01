using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebRestaurant.Models.ViewModels;

namespace WebRestaurant.Models
{
    public class Category
    {
        public Category() 
        {
            Status = true;
        }
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tên danh mục")]
        [MaxLength(30)]
        [DisplayName("Danh mục")]
        public string Name { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [DisplayName("Mô tả")]
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
