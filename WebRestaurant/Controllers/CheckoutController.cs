using WebRestaurant.Models.ViewModels;
using WebRestaurant.Models;
using WebRestaurant.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Utility.Helpers;
using Stripe.Checkout;

namespace WebRestaurant.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CheckoutController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            ShoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.
					GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
				OrderHeader = new OrderHeader()
			};
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.FirstName + " " + ShoppingCartVM.OrderHeader.ApplicationUser.LastName;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            foreach (var item in ShoppingCartVM.ShoppingCartList)
			{
				item.Price = item.Product.Price;
				item.Total = item.Quantity * item.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += item.Total;
			}
			return View(ShoppingCartVM);
		}
		[HttpPost]
		[ActionName("Index")]
		public IActionResult IndexPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart
			.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");
            foreach (var item in ShoppingCartVM.ShoppingCartList)
			{
				item.Price = item.Product.Price;
				item.Total = item.Quantity * item.Price;
                ShoppingCartVM.OrderHeader.OrderTotal += item.Total;
            }
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusCOD;
			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			//cập nhật chi tiết
			foreach (var item in ShoppingCartVM.ShoppingCartList)
			{
				OrderDetail detail = new()
				{
					ProductId = item.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = item.Price,
					Count = item.Quantity,
				};
				_unitOfWork.OrderDetail.Add(detail);
				_unitOfWork.Save();
			}
		
			if(ShoppingCartVM.OrderHeader.PaymemtMethod == "Online")
			{
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"Checkout/Confirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "Checkout/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price),
                            Currency = "vnd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Quantity
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }               
			return RedirectToAction(nameof(Confirmation), new { id = ShoppingCartVM.OrderHeader.Id });
		}
		public IActionResult Confirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
			if(orderHeader.PaymemtMethod == "Online") {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusPending, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }          
            EmailSenderHelper.SendEmail("dulionel27@gmail.com", orderHeader.ApplicationUser.Email, "Đơn hàng",
				$"<p>Bạn đã đặt một đơn hàng mới #{orderHeader.Id}</p>");
		
			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			HttpContext.Session.Clear();
			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();
			return View();
		}

	}
}
