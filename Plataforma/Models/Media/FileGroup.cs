using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models.Media;
[Table("FileGroups")]
public class FileGroup {
    public int Id { get; set; }
    public string Guid { get; set; } = "";
    public virtual List<File> Files { get; set; } = new();
    public DateTime CreationTime { get; set; }
}