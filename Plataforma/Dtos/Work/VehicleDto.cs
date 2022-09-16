using Plataforma.Models.Work;

namespace Plataforma.Dtos.Work
{
    public class VehicleDto
    {
        public string LicensePlate { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public int? PivotNumber { get; set; }
    }
}
