using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }  
        public DbSet<ProductSubImg> ProductSubImgs { get; set; } 
        public DbSet<ProductColor> ProductColors { get; set; } 
        public DbSet<Brand> Brands { get; set; } 
        public DbSet<Catgeory> Categories { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-9Q6DBR4\\SQL22; Initial Catalog=E_Commerce;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
        }
    }
}