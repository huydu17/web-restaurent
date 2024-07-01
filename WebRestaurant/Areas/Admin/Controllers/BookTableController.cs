using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Models;
using WebRestaurant.Utility;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BookTableController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotyfService _notyf;
        public BookTableController(IUnitOfWork unitOfWork, INotyfService notyf)
        {
            _unitOfWork = unitOfWork;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            var booktable = _unitOfWork.BookTable.GetAll().OrderByDescending(x=>x.CreateDate).ToList();
            return View(booktable);
        }
        public IActionResult Details(int id)
        {
            var booktable = _unitOfWork.BookTable.Get(x => x.Id == id);
            return View(booktable);
        }
        public IActionResult Confirm(int id)
        {
            _unitOfWork.BookTable.UpdateStatus(id, SD.StatusApproved);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { id = id });
        }
        public IActionResult Cancel(int id)
        {
            _unitOfWork.BookTable.UpdateStatus(id, SD.StatusCancelled);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { id = id });
        }
        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<BookTable> booktables = _unitOfWork.BookTable.GetAll().OrderBy(x=>x.CreateDate).ToList();
            return Json(new { data = booktables });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var booktable = _unitOfWork.BookTable.Get(x => x.Id == id);
            _unitOfWork.BookTable.Remove(booktable);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xoá đơn đặt bàn thành công!" });
        }
        #endregion
    }
}
