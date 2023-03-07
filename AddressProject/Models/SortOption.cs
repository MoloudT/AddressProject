using System.Text.Json.Serialization;

namespace AddressProject.Models
{
    public class SortOption
    {
            public SortOrderType? SortOrder { get; set; }
            public SortByType? SortBy { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortOrderType
    {
            Asc,
            Desc
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortByType
    {
            Id,
            Street,
            HouseNumber,
            ZipCode,
            City,
            Country
    }

}
