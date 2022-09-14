using Microsoft.EntityFrameworkCore;
using Plataforma.Models.Identity;
using System;

namespace Plataforma.Models.AutoLog;

[Owned]
public class AutoLogUpdate {
    public static readonly TimeSpan MaxTimeSpan = new(365, 0, 0, 0, 0);
    public DateTime Time { get; set; } = DateTime.Now;
    public virtual User User { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; } = "";
}