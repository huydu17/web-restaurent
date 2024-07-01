using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly INotyfService _notyf;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterVM registerVM { get; set; }
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IUnitOfWork unitOfWork, INotyfService notyf)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _notyf = notyf;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user != null)
            {
                var passCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(loginVM.Password,"Mật khẩu không đúng");
                return View(loginVM);
            }
			ModelState.AddModelError(loginVM.UserName, "Tải khoản không tồn tại");
            _notyf.Success("Đăng nhập thành công");
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            var userVM = new RegisterVM()
            {
                ListRole = _unitOfWork.ApplicationRole.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(userVM);
        }
        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterPost(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                registerVM = new RegisterVM()
                {
                    ListRole = _unitOfWork.ApplicationRole.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    })
                };
                return View(registerVM);
            }
                
            var role = _unitOfWork.ApplicationRole.Get(x => x.Id == registerVM.RoleId);
            var user = await _userManager.FindByNameAsync(registerVM.UserName);
            if (user != null)
            {
                ModelState.AddModelError("username","Tài khoản đã tồn tại");
                registerVM = new RegisterVM()
                {
                    ListRole = _unitOfWork.ApplicationRole.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    })
                };
                return View(registerVM);
            }
            var appUser = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Email = registerVM.Email,
                DoB = registerVM.DoB,
                CreateDate = DateTime.Now,
                UserName = registerVM.UserName,
                PhoneNumber = registerVM.PhoneNumber,
                Address = registerVM.Address,
            };
            var newuser = await _userManager.CreateAsync(appUser,registerVM.Password);
            if (newuser.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, role.Name);
            }
            _notyf.Success("Đăng ký thành công");
            return RedirectToAction("Index","User");   
        }      
    }
}
