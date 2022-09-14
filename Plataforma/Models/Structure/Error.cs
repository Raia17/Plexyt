using Plataforma.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models.Structure;

[Table("Errors")]
public class Error {
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Message { get; set; } = "";
    public string Module { get; set; } = "";
    public int? UserId { get; set; }
    public virtual User User { get; set; }
}