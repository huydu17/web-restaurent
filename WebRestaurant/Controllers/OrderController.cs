using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe;

namespace WebSoccer.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        public IActionResult CancelOrder()
        {
            var orderheader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
            if (orderheader.PaymemtMethod == "Online")
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
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }
        #region API CALL
        public IActionResult GetAll(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

            IEnumerable<OrderHeader> order = _unitOfWork.OrderHeader
                .GetAll(x=>x.ApplicationUserId == userId, includeProperties: "ApplicationUser").OrderByDescending(x=>x.OrderDate);

            switch (status)
            {
                case "pending":
                    order = order.Where(u => u.OrderStatus == SD.StatusPending).OrderBy(x => x.OrderDate);
                    break;
                case "approved":
                    order = order.Where(u => u.OrderStatus == SD.StatusApproved);
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
