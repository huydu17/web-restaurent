using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebRestaurant.DataAccess.Repository;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;
        public BlogController(IUnitOfWork unitOfWork, INotyfService notyf, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _notyf = notyf;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var blogs = _unitOfWork.Blog.GetAll().OrderByDescending(x=>x.CreateDate).ToList();
            return View(blogs);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Blog blog, IFormFile? file)
        {

            if (!ModelState.IsValid)
            {
                return View(blog);
            }
            string wwwroot = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string categoryPath = Path.Combine(wwwroot, @"images\blogs");
                using (var fileStream = new FileStream(Path.Combine(categoryPath, filename), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                blog.ImageUrl = @"\images\blogs\" + filename;
            }
            _unitOfWork.Blog.Add(blog);
            _unitOfWork.Save();
            _notyf.Success("Tạo Blog thành công");
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var blogDb = _unitOfWork.Blog.Get(u => u.Id == id);
            if (blogDb == null)
            {
                return NotFound();
            }
            return View(blogDb);
        }
        [HttpPost]
        public IActionResult Edit(Blog blog, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return View(blog);
            }
            string wwwroot = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string categoryPath = Path.Combine(wwwroot, @"images\blogs");
                if (!string.IsNullOrEmpty(blog.ImageUrl))
                {
                    var oldPath = Path.Combine(wwwroot, blog.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(categoryPath, filename), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                blog.ImageUrl = @"\images\blogs\" + filename;
            }
            _unitOfWork.Blog.Update(blog);
            _unitOfWork.Save();
            _notyf.Success("Cập nhật blog thành công");
            return View(nameof(Index));
        }
        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var blogs = _unitOfWork.Blog.GetAll().OrderByDescending(x => x.CreateDate).ToList();
            return Json(new { data = blogs });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var blog = _unitOfWork.Blog.Get(x => x.Id == id);
			string productPath = @"images\blogs\";
			string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

			if (Directory.Exists(finalPath))
			{
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths)
				{
					System.IO.File.Delete(filePath);
				}

				Directory.Delete(finalPath);
			}
			_unitOfWork.Blog.Remove(blog);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xoá blog thành công!" });
        }
        #endregion
    }
}
