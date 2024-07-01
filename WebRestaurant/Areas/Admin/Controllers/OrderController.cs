using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly INotyfService _notyf;
        [BindProperty]
		public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork, INotyfService notyf)
        {
            _unitOfWork = unitOfWork;
            _notyf = notyf;
        }

        public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
			OrderVM orderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(x=>x.Id == orderId,includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(x=>x.OrderHeaderId == orderId,includeProperties:"Product")
			};
            return View(orderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.Phone = OrderVM.OrderHeader.Phone;
            orderHeaderFromDb.Address = OrderVM.OrderHeader.Address;
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            _notyf.Success("Cập nhật đơn hàng thành công");
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            var orderheader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
            orderheader.OrderStatus = SD.StatusApproved;
            _unitOfWork.OrderHeader.Update(orderheader);
            _unitOfWork.Save();
            _notyf.Success("Đã xác nhận");
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }
        [HttpPost]
        public IActionResult ShippingOrder()
        {
            var orderheader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
            orderheader.OrderStatus = SD.StatusInTransit;
            _unitOfWork.OrderHeader.Update(orderheader);
            _unitOfWork.Save();
            _notyf.Success("Đang vận chuyển");
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult ShippedOrder()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped, SD.PaymentStatusApproved);
            _unitOfWork.Save();
            _notyf.Success("Hoàn tất đơn hàng");
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult CancelOrder()
       {
            var orderheader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
            if(orderheader.PaymemtMethod == "Online")
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderheader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStatus(orderheader.Id, SD.StatusCancelled, SD.PaymentRefund);
            }
            _unitOfWork.OrderHeader.UpdateStatus(orderheader.Id, SD.StatusCancelled);
            _unitOfWork.Save();
            _notyf.Success("Đã huỷ đơn hàng");
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        #region API CALL
        [HttpGet]
		public IActionResult GetAll(string status)
		{
            IEnumerable<OrderHeader> order = _unitOfWork.OrderHeader
                .GetAll(includeProperties: "ApplicationUser").OrderByDescending(x=>x.OrderDate);   
            switch (status)
            {
                case "pending":
                    order = order.Where(u => u.OrderStatus == SD.StatusPending).OrderBy(x => x.OrderDate);
                    break;
                case "approved":
                    order = order.Where(u => u.OrderStatus == SD.StatusApproved).OrderBy(x => x.OrderDate);
                    break;
                case "intransit":
                    order = order.Where(u => u.OrderStatus == SD.StatusInTransit).OrderBy(x => x.OrderDate);
                    break;
                case "shipped":
                    order = order.Where(u => u.OrderStatus == SD.StatusShipped).OrderBy(x => x.OrderDate);
                    break;
                case "cancelled":
                    order = order.Where(u => u.OrderStatus == SD.StatusCancelled).OrderBy(x => x.OrderDate);
                    break;
                default:
                    break;
            }
            return Json(new { data = order });
        }
		#endregion
	}
}
