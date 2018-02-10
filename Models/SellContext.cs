using Microsoft.EntityFrameworkCore;
 
namespace sellwalker.Models
{
    public class SellContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public SellContext(DbContextOptions<SellContext> options) : base(options) { }
        //<User> stands for Module , Users stands for table name of db table
        public DbSet<User> Users {get; set;}
        public DbSet<Product> Products {get; set;}
        public DbSet<Order> Orders {get; set;}
    }
}