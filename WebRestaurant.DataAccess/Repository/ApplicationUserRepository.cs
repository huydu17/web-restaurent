using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRestaurant.DataAccess.Repository;
using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using WebRestaurant.Models;

namespace WebRestaurant.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser> ,IApplicationUserRepository
    {
        private ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }
        public void Update(ApplicationUser obj)
        {
			_context.ApplicationUsers.Update(obj);
		}

    }
}
