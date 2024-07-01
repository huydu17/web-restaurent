using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;

namespace WebRestaurant.ViewComponents
{
    public class GetNameViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public GetNameViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }
    }
}
