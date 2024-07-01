using Microsoft.AspNetCore.Mvc;
using WebRestaurant.DataAccess.Repository.IRepository;

namespace WebRestaurant.Controllers
{
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BlogController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var blogs = _unitOfWork.Blog.GetAll().Where(x=>x.Status==true).ToList();
            return View(blogs);
        }
        public IActionResult Details(int id) 
        {
            var blog = _unitOfWork.Blog.Get(x=>x.Id==id);
            return View(blog);
        }
    }
}
