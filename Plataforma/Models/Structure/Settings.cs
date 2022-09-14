using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models.Structure;

[Table("Settings")]
public class Settings {
    public int Id { get; set; }
    public string UsageRules { get; set; } = "";
    public GlobalSettings GlobalSettings { get; set; }
}

[Owned]
public class GlobalSettings {
    public int SettingsId { get; set; }
    [Required]
    public string TestProperty { get; set; } = "";
}
