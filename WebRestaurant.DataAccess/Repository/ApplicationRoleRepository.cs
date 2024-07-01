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
    public class ApplicationRoleRepository : Repository<ApplicationRole>, IApplicationRoleRepository
    {
        private ApplicationDbContext _db;
        public ApplicationRoleRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationRole applicationRole)
        {
            _db.ApplicationRoles.Update(applicationRole);
        }
    }
}
