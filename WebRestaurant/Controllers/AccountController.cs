using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Net.WebSockets;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.SignalR;
using WebRestaurant.Hubs;

namespace WebRestaurant.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotyfService _notyf;
        public RegisterVM registerVM { get; set; }
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUnitOfWork unitOfWork, INotyfService notyf)
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
                var islockedout = await _userManager.IsLockedOutAsync(user);
                if (islockedout)
                {
                    ViewBag.Error = "Tài khoản của bạn đã bị khoá!";
                }
                var passCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
          
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng";
                }
                return View(loginVM);
            }
            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng";
            return View(nameof(Login));
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM, [FromServices] IHubContext<ChatHub> chat)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
    
            var role = "Admin";
            var user = await _userManager.FindByNameAsync(registerVM.UserName);
            if (user != null)
            {
                ModelState.AddModelError("username", "Tài khoản đã tồn tại");
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
            var newuser = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (newuser.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, "customer");
                await _signInManager.SignInAsync(appUser, isPersistent: false);
                var conversation = new Conversation()
                {
                    UserId = appUser.Id,
                    Name = registerVM.FirstName + " " + registerVM.LastName,
                };
                _unitOfWork.Conversation.Add(conversation);
                _unitOfWork.Save();
                await chat.Clients.All.SendAsync("LoadConversation");
                return RedirectToAction("Index", "Home");
            }
            return View(nameof(Register));
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
            return View(user);
        }
        [HttpGet]
        public IActionResult Update()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
            var updateVm = new UpdateProfileVM()
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
            };
            return View(updateVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateProfileVM vm)
        {
            if (!ModelState.IsValid) { return View(); }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _unitOfWork.ApplicationUser.Get(x=>x.Id == userId);
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Address= vm.Address;
            user.PhoneNumber = vm.PhoneNumber;
            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Save();
            _notyf.Success("Cập nhật thông tin thành công!");
            return RedirectToAction("Profile","Account");
        }

        public async Task<IActionResult> ChangePass()
        {            
            return View(new ChangePassVM());
        }
        [HttpPost]
        public async Task<IActionResult> ChangePass(ChangePassVM vm)
        {
            if (!ModelState.IsValid) { return View(); }
            var user = await _userManager.GetUserAsync(User);
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, vm.CurrentPass);
            if (!isCurrentPasswordValid)
            {
                ModelState.AddModelError("currentpass", "Mật khẩu hiện tại không đúng.");
                return View();
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, vm.CurrentPass, vm.NewPass);
            if (!changePasswordResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi thay đổi mật khẩu.");
                return View();
            }
            _notyf.Success("Đổi thành công!");
            return RedirectToAction("Profile", "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");   
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
