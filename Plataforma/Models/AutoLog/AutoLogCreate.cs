using Microsoft.EntityFrameworkCore;
using Plataforma.Models.Identity;
using System;

namespace Plataforma.Models.AutoLog;

[Owned]
public class AutoLogCreate {
    public DateTime CreationTime { get; set; }
    public int? UserId { get; set; }
    public virtual User User { get; set; }
}