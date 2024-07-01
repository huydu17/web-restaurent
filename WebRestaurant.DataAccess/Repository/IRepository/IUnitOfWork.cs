using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IApplicationRoleRepository ApplicationRole { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IProductImageRepository ProductImage { get; }
        ICommentRepository Comment { get; }
        IMessageRepository Message { get; } 
        IConversationRepository Conversation { get; }   
        IBlogRepository Blog { get; }
        IBookTableRepository BookTable { get; }
        void Save();
    }
}
