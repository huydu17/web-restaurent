using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        [Required(ErrorMessage ="Vui lòng nhập mô tả")]
        public string Description { get;set; }
    }
}
