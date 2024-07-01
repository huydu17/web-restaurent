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
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        private readonly ApplicationDbContext _context;
        public BlogRepository(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }

        public void Update(Blog blog)
        {
            _context.Blogs.Update(blog);
        }
    }
}
