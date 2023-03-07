using AddressProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressProject.DB
{
    public class AddressDataContex:DbContext
    {
        protected readonly IConfiguration Configuration;
        public AddressDataContex(IConfiguration configuration)
        {
            Configuration = configuration;
        }
       
         protected override void OnConfiguring(DbContextOptionsBuilder options)
         {
             options.UseInMemoryDatabase(Configuration.GetConnectionString("WebApiDatabase"));
          
         }
        public DbSet<Address> Address { get; set; } = null;

    }
}


