using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Plataforma.Helpers;

public static class FileHelper {
    public static Models.Media.File SaveFile(IFormFile file) {
        if (file == null || file.Length == 0) return null;

        var fileByteArray = "";

        var extension = Path.GetExtension(file.FileName);

        using var ms = new MemoryStream();
        file.CopyTo(ms);
        var fileBytes = ms.ToArray();
        fileByteArray = Convert.ToBase64String(fileBytes);

        var filenew = new Models.Media.File() {
            Filename = file.FileName,
            Extension = extension,
            Size = file.Length,
            ContentType = file.ContentType,
            Binary = fileByteArray
        };
        return filenew;
    }

}