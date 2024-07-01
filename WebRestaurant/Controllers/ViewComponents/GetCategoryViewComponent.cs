using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebRestaurant.ViewComponents
{
    public class GetCategoryViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCategoryViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }
    }
}
