using System;
using System.Collections.Generic;

namespace Plataforma.Models.Work
{
    public class WorkSheet
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public int Kms { get; set; }
        public string WorkDone { get; set; }
        public string WorkTodo { get; set; }
        public virtual List<Material> Materials { get; set; } = new();
        }
}
