using Plataforma.Models.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma.Models.Media;

[Table("Files")]

public class File : ISortable {
    public virtual FileGroup FileGroup { get; set; }
    public int? FileGroupId { get; set; }

    public int Id { get; set; }
    public int Order { get; set; }
    public long Size { get; set; } // Size in bytes
    public string Extension { get; set; } = "";
    public string Binary { get; set; } = "";
    public string Filename { get; set; } = ""; // eg. abc.pdf
    public string ContentType { get; set; } = ""; // eg. text/html
}
