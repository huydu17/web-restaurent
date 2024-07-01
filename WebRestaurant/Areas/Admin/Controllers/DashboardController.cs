using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Utility;
using WebRestaurant.Utility.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class DashboardController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
		{
            var orderheaders = _unitOfWork.OrderHeader.GetAll().Where(x=>x.OrderStatus == SD.StatusShipped);
            double total = 0;
            foreach(var order in orderheaders) {
                total += order.OrderTotal;
            }
            var totalusers = _unitOfWork.ApplicationUser.GetAll().Count();
            var productsold = _unitOfWork.OrderDetail.GetAll();
            int totalProducts = 0;
            foreach (var order in productsold)
            {
                totalProducts += order.Count;
            }
            var orderheader = _unitOfWork.OrderHeader.GetAll().Where(x => x.OrderStatus == SD.StatusShipped).Count();
            ViewBag.TotalOrder = orderheader;
            ViewBag.Total = total.ToVnd();
            ViewBag.TotalUsers = totalusers;
            ViewBag.Products = totalProducts;
			return View();
		}
	}
}
