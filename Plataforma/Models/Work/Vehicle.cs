namespace Plataforma.Models.Work
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public int PivotNumber { get; set; }
    }
}
