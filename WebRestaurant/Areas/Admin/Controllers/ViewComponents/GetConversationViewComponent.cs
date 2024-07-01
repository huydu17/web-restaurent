using Microsoft.AspNetCore.Mvc;
using WebRestaurant.DataAccess.Repository.IRepository;

namespace WebRestaurant.Areas.Admin.Controllers.ViewComponents
{
    public class GetConversationViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetConversationViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var conversations = _unitOfWork.Conversation.GetAll(includeProperties:"Messages");
            return View(conversations);
        }
    }
}
