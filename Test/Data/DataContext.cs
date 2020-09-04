using Microsoft.EntityFrameworkCore;
using Test.Model;

namespace Test.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
    }
}