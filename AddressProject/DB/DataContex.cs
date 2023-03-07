using AddressProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressProject.DB
{
    public class DataContex:DbContext
    {
        protected readonly IConfiguration Configuration;
        public DataContex(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public DbSet<Address> Address { get; set; }

    }
}

