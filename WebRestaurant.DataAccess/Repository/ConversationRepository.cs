using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using WebRestaurant.Models;

namespace WebRestaurant.DataAccess.Repository
{
    public class ConversationRepository : Repository<Conversation>, IConversationRepository
    {
        private readonly ApplicationDbContext _db;
        public ConversationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Conversation conversation)
        {
            _db.Conversations.Update(conversation);
        }
    }
}
