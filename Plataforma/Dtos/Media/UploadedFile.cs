using Microsoft.AspNetCore.Http;
using Plataforma.Models.Media;

namespace Plataforma.Dtos.Media;
public class UploadedFile {
    public IFormFile File { get; set; }
    public File FileModel { get; set; }
    public bool Delete { get; set; } = false;
}
