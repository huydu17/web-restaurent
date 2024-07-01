using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRestaurant.Models;

namespace WebRestaurant.DataAccess.Repository.IRepository
{
    public interface IBookTableRepository : IRepository<BookTable>
    {
        void Update(BookTable booktable);
        void UpdateStatus(int id, string bookstatus);
    }
}
