using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }  
        public DbSet<ProductSubImg> ProductSubImgs { get; set; } 
        public DbSet<ProductColor> ProductColors { get; set; } 
        public DbSet<Brand> Brands { get; set; } 
        public DbSet<Catgeory> Categories { get; set; } 

 
    }
}