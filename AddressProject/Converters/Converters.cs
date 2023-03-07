using AddressProject.Entities;
using AddressProject.Models;

namespace AddressProject.Converter
{
    public static class Converters
    {
        public static AddressDTO ToAddressDTO(this Address address) =>
           new AddressDTO
           {
               Id = address.Id,
               Street = address.Street,
               HouseNumber = address.HouseNumber,
               ZipCode = address.ZipCode,
               City = address.City,
               Country = address.Country,

           };
        public static Address ToAddress(this AddressDTO addressDTO) =>
            new Address
            {
                Id = addressDTO.Id,
                Street = addressDTO.Street,
                HouseNumber = addressDTO.HouseNumber,
                ZipCode = addressDTO.ZipCode,
                City = addressDTO.City,
                Country = addressDTO.Country,

            };
    }
}
