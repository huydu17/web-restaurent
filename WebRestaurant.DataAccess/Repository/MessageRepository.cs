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
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public readonly ApplicationDbContext _db;
        public MessageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Message message)
        {
            _db.Messages.Update(message);
        }
    }
}
