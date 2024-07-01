using WebRestaurant.Models.ViewModels;
using WebRestaurant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using AspNetCoreHero.ToastNotification.Abstractions;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Utility;

namespace WebRestaurant.Controllers
{
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly INotyfService _notyf;
		private int PageSize = SD.PageSize;
		public CategoryController(IUnitOfWork unitofwork, INotyfService notyf)
		{
			_unitOfWork = unitofwork;
			_notyf = notyf;
		}
		[HttpGet]
		public IActionResult Index(int categoryId, int productpage = 1)
		{
			IEnumerable<Category> categories = _unitOfWork.Category.GetAll().ToList();
			IEnumerable<Product> products = _unitOfWork.Product.GetAll(x => x.CategoryId == categoryId, includeProperties: "Category,ProductImages")
							.Skip((productpage - 1) * PageSize)
							.Take(PageSize)
							.ToList();

			ProductCategoryVM productVM = new()
			{
				Products = products,
				Categories = categories,
				PageInfo = new PageInfo()
				{
					TotalItems = _unitOfWork.Product.GetAll(x => x.CategoryId == categoryId, includeProperties: "Category,ProductImages").Count(),
					ItemsPerPage = PageSize,
					CurrentPage = productpage,
				}
			};
			ViewBag.CategoryId = categoryId;
			ViewBag.Category = _unitOfWork.Category.Get(x => x.Id == categoryId).Name;
			return View(productVM);
		}

		[HttpPost]
		public IActionResult GetFilters(int categoryId,string sort)
		{
			var categories = _unitOfWork.Category.GetAll().ToList();
			var products = _unitOfWork.Product.GetAll(x => x.CategoryId == categoryId, includeProperties: "Category,ProductImages");
		
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
	}
}
