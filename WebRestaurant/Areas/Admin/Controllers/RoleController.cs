using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Policy;
using WebRestaurant.DataAccess.Repository;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _appRole;
        private readonly INotyfService _notyf;
        public RoleController(RoleManager<ApplicationRole> appRole)
        {
            _appRole = appRole;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ApplicationRole> roles = _appRole.Roles.ToList();
            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleVM role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }
            if (!_appRole.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                 _appRole.CreateAsync(new ApplicationRole { Name = role.Name, Description = role.Description }).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index", "Role");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var newroleId = id.ToString();
            var roles = await _appRole.FindByIdAsync(newroleId);
            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoleVM roleVM)
        {
            if (!ModelState.IsValid) { return View(roleVM); }
            var roledB = await _appRole.FindByNameAsync(roleVM.Name);
            roledB.Description = roleVM.Description;
            await _appRole.UpdateAsync(roledB);
            return RedirectToAction("Index");
        }
        #region APICALL
        public IActionResult GetAll()
        {
            List<ApplicationRole> roles = _appRole.Roles.ToList();
            return Json(new { data = roles });
        }
        #endregion
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _appRole.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            await _appRole.DeleteAsync(role);
            return Json(new { success = true, message = "Xoá thành công!" });
        }

    }
}
