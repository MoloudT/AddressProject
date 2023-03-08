using AddressProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace AddressProject.DB
{
#pragma warning disable CS1591
    public class AddressDataContex:DbContext
    {
        protected readonly IConfiguration Configuration;
        public AddressDataContex(IConfiguration configuration)
        {
            Configuration = configuration;
        }
       
         protected override void OnConfiguring(DbContextOptionsBuilder options)
         {
             options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));  
        }
        public DbSet<Address> Address { get; set; }

    }
#pragma warning disable CS1591
}


