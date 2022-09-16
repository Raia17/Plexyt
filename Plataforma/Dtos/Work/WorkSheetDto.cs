using Plataforma.Models.Work;
using System;
using System.Collections.Generic;

namespace Plataforma.Dtos.Work
{
    public class WorkSheetDto
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int? Kms { get; set; }
        public string WorkDone { get; set; }
        public string WorkTodo { get; set; }
        public List<int> MaterialsId { get; set; } = new();
        public List<int> Amounts { get; set; } = new();
        public List<Material> Materials { get; set; } = new();
    }
}
