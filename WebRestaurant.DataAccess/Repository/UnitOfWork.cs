using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IApplicationRoleRepository ApplicationRole { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }  
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public ICommentRepository Comment { get; private set; }
        public IMessageRepository Message { get; private set; } 
        public IConversationRepository Conversation { get; private set; }
        public IBlogRepository Blog { get; private set; }
        public IBookTableRepository BookTable { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
            ApplicationRole = new ApplicationRoleRepository(_db);
            ProductImage = new ProductImageRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);   
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);   
            Comment = new CommentRepository(_db);
            Message = new MessageRepository(_db);
            Conversation = new ConversationRepository(_db);
            Blog = new BlogRepository(_db);
            BookTable = new BookTableRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
