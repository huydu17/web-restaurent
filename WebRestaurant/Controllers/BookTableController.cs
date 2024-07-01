using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Models;
using WebRestaurant.DataAccess.Repository.IRepository;
using AspNetCoreHero.ToastNotification.Abstractions;
using WebRestaurant.Utility;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebRestaurant.Controllers
{
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
            var booktable = _unitOfWork.BookTable.GetAll().ToList();
            return View(booktable);  
        }
        [Authorize]
        public IActionResult Book()
        {
            List<SelectListItem> numOfPepple = new List<SelectListItem>();
            numOfPepple.Add(new SelectListItem() { Text = "1 người", Value = "1" });
            numOfPepple.Add(new SelectListItem() { Text = "2 người", Value = "2" });
            numOfPepple.Add(new SelectListItem() { Text = "3 người", Value = "3" });
            numOfPepple.Add(new SelectListItem() { Text = "4 người", Value = "4" });
            numOfPepple.Add(new SelectListItem() { Text = "5 người", Value = "5" });
            numOfPepple.Add(new SelectListItem() { Text = "Khác", Value = "6" });
            ViewBag.NumberOfPeople = numOfPepple;
            return View();
        }
        [HttpPost]
        public IActionResult Book(BookTableVM vm)
        {
            if (!ModelState.IsValid) {
                List<SelectListItem> numOfPepple = new List<SelectListItem>();
                numOfPepple.Add(new SelectListItem() { Text = "1 người", Value = "1" });
                numOfPepple.Add(new SelectListItem() { Text = "2 người", Value = "2" });
                numOfPepple.Add(new SelectListItem() { Text = "3 người", Value = "3" });
                numOfPepple.Add(new SelectListItem() { Text = "4 người", Value = "4" });
                numOfPepple.Add(new SelectListItem() { Text = "5 người", Value = "5" });
                numOfPepple.Add(new SelectListItem() { Text = "Khác", Value = "6" });
                return View(vm); 
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var booktable = new BookTable()
            {
                UserId = userId,
                Name = vm.Name,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                ArrivalDatetime = DateTime.ParseExact($"{vm.ArrivalDate} {vm.ArrivalTime}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                NumberOfPeople = vm.NumberOfPeople,
                Message = vm.Message,
                Status = SD.StatusPending
            };
            _unitOfWork.BookTable.Add(booktable);
            _unitOfWork.Save();
            _notyf.Success("Đặt bàn thành công!");
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Details (int id)
        {
            var booktable = _unitOfWork.BookTable.Get(x=>x.Id==id);
            return View(booktable);
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<BookTable> booktables = _unitOfWork.BookTable.GetAll(x=>x.UserId==userId).OrderByDescending(x=>x.CreateDate).ToList();
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
