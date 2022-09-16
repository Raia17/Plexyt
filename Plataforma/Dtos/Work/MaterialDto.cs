using Plataforma.Models.Work;
using System.Collections.Generic;

namespace Plataforma.Dtos.Work
{
    public class MaterialDto
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Ref { get; set; }
        public List<WorkSheet> WorkSheets { get; set; }
    }
}
