namespace AddressProject.Models
{
    public class Position
    {
        public double lat { get; set; }
        public double lon { get; set; }

        public static double distance(Position from, Position other)
                => Math.Acos(Math.Sin(from.lat) * Math.Sin(other.lat)
                + Math.Cos(from.lat) * Math.Cos(other.lat)
                * Math.Cos(other.lon - from.lon)) * 6371000;
    }
}
