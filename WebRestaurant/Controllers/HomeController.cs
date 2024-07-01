using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebRestaurant.Models.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using WebRestaurant.Utility.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace WebRestaurant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotyfService _notyf;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, INotyfService notyf)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _notyf = notyf;
        }

        public IActionResult Index()
        {

            List<Product> latestProducts = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages")
                .OrderByDescending(x => x.CreateAt).Take(4).ToList();   
            return View(latestProducts);
        }
        public IActionResult Contact() {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(Contact contact)
        {
            if (!ModelState.IsValid) { return  View(contact); }
            var msg = "Tên: " + contact.Name + "<br>";
            msg += "Email: " + contact.Email + "<br>";
            msg += "Nội dung: " + contact.Message.Replace(Environment.NewLine, "<br>");
            EmailSenderHelper.SendEmail(contact.Email, "dulionel27@gmail.com", contact.Subject, msg);
            _notyf.Success("Gửi phản hồi thành công!");
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
	}
}