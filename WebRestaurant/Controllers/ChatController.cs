using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NuGet.Packaging.Signing;
using System.Security.Claims;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using WebRestaurant.Hubs;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using WebRestaurant.Utility;

namespace WebRestaurant.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ChatController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = Guid.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
            var coversation = _unitOfWork.Conversation.Get(x=>x.UserId == userId,includeProperties:"Messages");
            ViewBag.UserId = userId;
            return View(coversation);
        }

        [HttpPost("/Chat/SendMessage")]
        public async Task<IActionResult> SendMessage(int conversationId,string content, [FromServices] IHubContext<ChatHub> chat)
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
    }
}
