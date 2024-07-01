using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Net.WebSockets;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.Hubs;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ChatController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var adminId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var conversation = _unitOfWork.Conversation.GetAll(includeProperties:"Messages").FirstOrDefault();
            ViewBag.SenderId = adminId;
            ViewBag.ConversationId = conversation.Id;
            return View(conversation);     
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int conversationId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var adminId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var conversation = _unitOfWork.Conversation.Get(x=>x.Id == conversationId,includeProperties:"Messages");
            var user = _unitOfWork.ApplicationUser.Get(x => x.Id == conversation.UserId);
            ViewBag.SenderId = adminId;
            ViewBag.FullName = user.FirstName + " " + user.LastName;
       
            return View(conversation);
        }
        [HttpGet]
        public async Task<IActionResult> GetConversation([FromServices] IHubContext<ChatHub> chat)
        {
            var conversation = _unitOfWork.Conversation.GetAll();
            await chat.Clients.All.SendAsync("LoadConversation");
            return Ok(conversation);
        }

        [HttpPost("Admin/Chat/SendMessage")]
        public async Task<IActionResult> SendMessage(int conversationId, string content, [FromServices] IHubContext<ChatHub> chat)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var Message = new Message()
            {
                ConversationId = conversationId,
                SenderId = userId,
                Content = content,
                Timestamp = DateTime.Now,
            };
            _unitOfWork.Message.Add(Message);
            _unitOfWork.Save();
            await chat.Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    SenderId = Message.SenderId,
                    Content = Message.Content,
                    Timestamp = Message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss"),
                });
            return Ok();
        }
        [HttpPost]
        public IActionResult SendImage(int conversationId, IFormFile? file)
        {
            return View();
        }
    }
}
