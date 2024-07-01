using WebRestaurant.Models.ViewModels;
using WebRestaurant.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using AspNetCoreHero.ToastNotification.Abstractions;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Utility;

namespace eShopSoccer.Controllers
{
	public class SearchController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly INotyfService _notyf;
		private int PageSize = SD.PageSize;
		public SearchController(IUnitOfWork unitofwork, INotyfService notyf)
		{
			_unitOfWork = unitofwork;
			_notyf = notyf;
		}
		[HttpGet]
		public IActionResult Index(string keyword, int productpage = 1)
		{
			List<Category> categories;
			List<Product> products;
			ProductCategoryVM productVM;
			if (!string.IsNullOrEmpty(keyword))
			{
				categories = _unitOfWork.Category.GetAll().ToList();
				products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages")
								.Where(x => x.Name.ToLower().Contains(keyword.ToLower()))
								.Skip((productpage - 1) * PageSize)
								.Take(PageSize)
								.ToList();
				ViewBag.Keyword = keyword;
				productVM = new()
				{
					Products = products,
					Categories = categories,
					PageInfo = new PageInfo()
					{
						TotalItems = _unitOfWork.Product.GetAll()
								.Where(x => x.Name.Contains(keyword)).Count(),
						ItemsPerPage = PageSize,
						CurrentPage = productpage,
					}
				};
			}
			else
			{

				return RedirectToAction("Index","Product");

			}
			return View(productVM);
		}
}
}
