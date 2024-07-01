using WebRestaurant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebRestaurant.DataAccess.Configuration;

namespace WebRestaurant.DataAcess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products{ get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BookTable> BookTables { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Món khai vị", Status=true ,Description="1"}
                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Khai vị 1",  
                    ShortDescription = "Khai vị",
                    Description = "Khai vị khai vị",
                    Status = true,
                    Price = 100000,
                    CategoryId = 1,
                    CreateAt = DateTime.Now,
                });
   //         modelBuilder.Entity<IdentityUserClaim<Guid>>();
   //         modelBuilder.Entity<IdentityUserRole<Guid>>().HasKey(x => new { x.UserId,x.RoleId }) ;
			//modelBuilder.Entity<IdentityUserLogin<Guid>>().HasKey(x=>x.UserId);
			//modelBuilder.Entity<IdentityRoleClaim<Guid>>();
			//modelBuilder.Entity<IdentityUserToken<Guid>>().HasKey(x=>x.UserId);
			base.OnModelCreating(modelBuilder);
        }

    }
}
