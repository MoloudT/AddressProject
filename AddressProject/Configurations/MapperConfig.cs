using SQLitePCL;
using AutoMapper;
using AddressProject.Models;
using AddressProject.Entities;

namespace AddressProject.Configurations
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<Address, AddressDTO>().ReverseMap();
        }
    }
}
