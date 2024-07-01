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
	public class BookTableRepository : Repository<BookTable>, IBookTableRepository
	{
		private readonly ApplicationDbContext _context;
		public BookTableRepository(ApplicationDbContext db) : base(db)
		{
			_context = db;
		}
		public void Update(BookTable booktable)
		{
			_context.BookTables.Update(booktable);
		}

        public void UpdateStatus(int id, string bookstatus)
        {
            var orderFromDb = _context.BookTables.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.Status = bookstatus;
            }
        }
    }
}
