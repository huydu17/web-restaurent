using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using WebRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.DataAccess.Repository
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly ApplicationDbContext _db;
        public CommentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Comment comment)
        {
            _db.Comments.Update(comment);
        }
    }
}
