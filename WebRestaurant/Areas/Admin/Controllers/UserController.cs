using WebRestaurant.DataAccess.Repository;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    { 
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _context = context;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public IActionResult RoleManagement(Guid userId)
        {

            RoleManagementVM roleVM = new RoleManagementVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId),
                RoleList = _context.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                })
            };
            roleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId))
                    .GetAwaiter().GetResult().FirstOrDefault();
            return View(roleVM);
        }
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleVM)
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleVM.ApplicationUser.Id))
                    .GetAwaiter().GetResult().FirstOrDefault();
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleVM.ApplicationUser.Id);
            if (!(roleVM.ApplicationUser.Role == oldRole))
            {
				_unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }
    
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> users = _context.ApplicationUsers.OrderByDescending(x=>x.CreateDate).ToList();
            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            foreach(var user in users)
            {
                var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
            }
            return Json(new {data = users});
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] Guid id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if(user == null)
            {
                return Json(new { suceess = false, message = "Lỗi khi khoá/mở tài khoản" });
            }
            if(user.LockoutEnd!=null && user.LockoutEnd > DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Thành công!" });
        }


        #endregion
    }
}
