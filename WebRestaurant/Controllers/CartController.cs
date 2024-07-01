using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;
using WebRestaurant.Utility.Helpers;
using System.Xml.Linq;

namespace WebRestaurant.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }  

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId
                    , includeProperties: "Product"),
                OrderHeader = new()
            };
			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
			foreach (var items in shoppingCartVM.ShoppingCartList)
            {
				items.Product.ProductImages = productImages.Where(u => u.ProductId == items.Product.Id).ToList();
				items.Price = items.Product.Price;
				items.Total = items.Quantity * items.Price;
                shoppingCartVM.OrderHeader.OrderTotal += items.Total;
			}
			return View(shoppingCartVM);
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
			HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
			  .GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
			_unitOfWork.Save();
			return Redirect(Request.Headers["Referer"].ToString());
		}
        [HttpPost]
        public IActionResult Update(List<UpdateCartVM> carVm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            foreach (var item in carVm)
            {
                var cart = _unitOfWork.ShoppingCart.Get(x => x.Id == item.CartId);
                if (cart != null)
                {
                    cart.Quantity = item.Quantity;
                    _unitOfWork.ShoppingCart.Update(cart);
                    _unitOfWork.Save();
                }
            }
            var carts = _unitOfWork.ShoppingCart.GetAll();
            foreach (var item in carts)
            {
                var checkcart = _unitOfWork.ShoppingCart.GetAll(x => x.ProductId == item.ProductId).ToList();
                if (checkcart.Count > 1)
                {
                    var total = checkcart.Sum(x => x.Quantity);
                    var newcart = new ShoppingCart()
                    {
                        ApplicationUserId = userId,
                        ProductId = item.ProductId,
                        Quantity = total,
                        Price = item.Price,
                        Total = item.Quantity * item.Price
                    };
                    _unitOfWork.ShoppingCart.Add(newcart);
                    _unitOfWork.ShoppingCart.RemoveRange(checkcart);
                    _unitOfWork.Save();
                }
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart
                    .GetAll(u => u.ApplicationUserId == item.ApplicationUserId).Count());
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult WishList()
        {
            List<CartItemVM> carts = HttpContext.Session.Get<List<CartItemVM>>("GioHang");
            if (carts == null)
            {
                carts = new List<CartItemVM>();
            }
            foreach (var item in carts)
            {
                item.Product.ProductImages = _unitOfWork.ProductImage.GetAll(x => x.ProductId == item.Product.Id).ToList();
            }
            return View(carts);
        }
        public IActionResult RemoveWishList(string guidId)
        {
            Guid guid = Guid.Parse(guidId);
            List<CartItemVM> carts = HttpContext.Session.Get<List<CartItemVM>>("GioHang");
            carts.RemoveAll(x => x.GuidId == guid);
            HttpContext.Session.Set("GioHang", carts);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
