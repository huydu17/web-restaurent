using WebRestaurant.DataAcess.Data;
using WebRestaurant.Models;
using WebRestaurant.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRestaurant.DataAccess.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ApplicationDbContext _db;

		public DbInitializer(
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			ApplicationDbContext db)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_db = db;
		}
		public void Initialize()
		{
			try
			{
				if (_db.Database.GetPendingMigrations().Count() > 0)
				{
					_db.Database.Migrate();
				}
			}
			catch (Exception ex) { }
			if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new ApplicationRole { Name = SD.Role_Admin, Description = "Quản trị viên" }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new ApplicationRole { Name = SD.Role_Customer, Description = "Khách hàng" }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new ApplicationRole { Name = SD.Role_Employee, Description = "Nhân viên" }).GetAwaiter().GetResult();

				_userManager.CreateAsync(new ApplicationUser
				{
					FirstName = "Quản Trị Viên",
					LastName = "Admin",
					UserName = "Admin",
					Email = "dulionel27@gmail.com",
					Address = "Quảng Trị",
					PhoneNumber = "+84123456789",
				}, "Admin@@123").GetAwaiter().GetResult();
				ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == "Admin");
				_userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
			}
			return;
		}
	}
}
