using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using WebRestaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebRestaurant.Utility;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;
        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _notyf = notyf;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {

            if (!ModelState.IsValid)
            {
               return View(category);
            }
            _unitOfWork.Category.Add(category);
            _unitOfWork.Save();
            _notyf.Success("Tạo danh mục thành công");
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryDb = _unitOfWork.Category.Get(u => u.Id == id);      
            if (categoryDb == null)
            {
                return NotFound();
            }
            return View(categoryDb);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }        
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            _notyf.Success("Cập nhật danh mục thành công");
            return View(nameof(Index));
        }
        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().OrderByDescending(x=>x.UpdateAt).ToList();
            return Json(new {data = categories});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var cateogry = _unitOfWork.Category.Get(x=>x.Id == id);
            _unitOfWork.Category.Remove(cateogry);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xoá danh mục thành công!" });
        }
        #endregion
    }
}
