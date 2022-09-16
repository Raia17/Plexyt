using System.Collections.Generic;

namespace Plataforma.Models.Work
{
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Ref { get; set; }
        public virtual List<WorkSheet> WorkSheets { get; set; } = new();
    }
}
