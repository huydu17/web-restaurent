using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.AspNetCore.Http;
using WebRestaurant.Utility.Helpers;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using WebRestaurant.Utility;

namespace WebRestaurant.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotyfService _notyf;
        private int PageSize = SD.PageSize;
        public ProductController(IUnitOfWork unitofwork, INotyfService notyf)
        {
            _unitOfWork = unitofwork;
            _notyf = notyf;
        }
        public IActionResult Index(int productpage = 1)
        {
            var categories = _unitOfWork.Category.GetAll().ToList();
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages")
                            .Skip((productpage - 1) * PageSize)
                            .Take(PageSize)
                            .ToList();
            ProductCategoryVM productVM = new()
            {
                Products = products,
                Categories = categories,
                PageInfo = new PageInfo()
                {
                    TotalItems = _unitOfWork.Product.GetAll().Count(),
                    ItemsPerPage = PageSize,
                    CurrentPage = productpage,
                }
            };
            return View(productVM);
        }

		[HttpPost]
		public IActionResult GetFilters(string sort)
		{
			var categories = _unitOfWork.Category.GetAll().ToList();
			var products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
			switch (sort)
			{
				case "lowtohigh":
					{
						products = products.OrderBy(x => x.Price).ToList();
						break;
					}
				case "hightolow":
					{
						products = products.OrderByDescending(x => x.Price).ToList();
						break;
					}
				case "az":
					{
						products = products.OrderBy(x => x.Name).ToList();
						break;
					}
                default:
                {
                    products = products.ToList();
					break;
                }
				
			}	
			return PartialView("_ProductList", products);
		}
		[HttpGet]
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages"),
                Quantity = 1,
                ProductId = productId
            };
            var comment = _unitOfWork.Comment.GetAll(x=>x.ProductId == productId).ToList();
            var productDetails = new ProductDetailVM()
            {
                ShoppingCart = cart,            
                Comments = comment
            };
			return View(productDetails);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                ProductId = productId,
                Quantity = quantity,
            };
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            cart.ApplicationUserId = userId;
            var cartDb = _unitOfWork.ShoppingCart.Get(x => x.ApplicationUserId == userId
                                                        && x.ProductId == cart.ProductId);
            int currentCartQuantity = 0;
            if (cartDb != null)
            {
                cartDb.Quantity += cart.Quantity;
                _unitOfWork.ShoppingCart.Update(cartDb);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Save();
                currentCartQuantity = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count();
                HttpContext.Session.SetInt32(SD.SessionCart, currentCartQuantity);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        public IActionResult AddToWish(int productId, int quantity)
        {
            var product = _unitOfWork.Product.Get(x => x.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            if (HttpContext.Session.Get<List<CartItemVM>>("GioHang") == null)
            {
                List<CartItemVM> carts = new List<CartItemVM>();
                carts.Add(new CartItemVM()
                {
                    Product = product,
                    Quantity = quantity,
                });
                HttpContext.Session.Set("GioHang", carts);
            }
            else
            {
                List<CartItemVM> carts = HttpContext.Session.Get<List<CartItemVM>>("GioHang");
                int index = FindCartItemIndex(productId, carts);
                if (index == -1)
                {
                    carts.Add(new CartItemVM()
                    {
                        Product = product,
                        Quantity = quantity,
                    });
                }
                else
                {
                    carts[index].Quantity += quantity;
                }
                HttpContext.Session.Set("GioHang", carts);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        private int FindCartItemIndex(int productId, List<CartItemVM> carts)
        {
            for (var i = 0; i < carts.Count; i++)
            {
                if (carts[i].Product.Id == productId)
                {
                    return i;
                }
            }
            return -1;
        }

        [HttpPost]
        public IActionResult AddComment(int productId, string text, int rating = 1 )
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _unitOfWork.ApplicationUser.Get(x=>x.Id== userId);   
            if(!ModelState.IsValid) { return View(); }
            var newcomment = new Comment()
            {
                ApplicationUserId = userId,
                Text = text,
                ProductId = productId,
                Name = user.FirstName + " "+user.LastName,
                Rating = rating,
            };
            _unitOfWork.Comment.Add(newcomment);
            _unitOfWork.Save();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
