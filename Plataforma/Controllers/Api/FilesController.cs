using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Extensions;
using Plataforma.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Plataforma.Controllers.Api;

[AllowAnonymous]
public class FilesController : ApiController {

    public FilesController(ApplicationDbContext dbContext) : base(dbContext) { }


    [HttpGet("{id}/{name?}")]
    public async Task<IActionResult> FileRead(int id, string name) {
        if (!User.Identity?.IsAuthenticated ?? false)
            return BadRequest("Not authenticated");
        var file = await _dbContext.Files.Where(g => g.Id == id).Select(f => new { f.Binary, f.Filename, f.ContentType, f.Extension }).WithNoLockFirstOrDefaultAsync();
        if (file == null)
            return NotFound();
        var notDownloadable = new List<string> { ".pdf", ".jpg", ".jpeg", ".png", ".bmp" };
        return File(Convert.FromBase64String(file.Binary), file.ContentType, notDownloadable.Contains(file.Extension) ? null : file.Filename);
    }

    [HttpPost]
    public async Task<IActionResult> FileUpload(int fileGroupId, [Required] IFormFile file) {
        if (!User.Identity?.IsAuthenticated ?? false)
            return BadRequest("Not authenticated");
        try {
            try {
                Console.WriteLine(HttpContext.Request.Form.Count);
            } catch (BadHttpRequestException) {
                return BadRequest("O ficheiro introduzido é demasiado grande.");
            }

            var fileGroup = await _dbContext.FileGroups.WithNoLockFirstOrDefaultAsync(g => g.Id == fileGroupId);
            if (fileGroup == null) {
                return NotFound("File Group não encontrado.");
            }

            var fileModel = FileHelper.SaveFile(file);

            if (fileModel == null) {
                return BadRequest("Ocorreu um erro ao gravar o ficheiro.");
            }
            fileModel.FileGroupId = fileGroupId;

            fileModel.Order = fileGroup.Files.Select(i => i.Order).DefaultIfEmpty(0).Max() + 1;

            _dbContext.Files.Add(fileModel);
            await _dbContext.SaveChangesAsync();
            return View("../Shared/Media/_File", fileModel);
        } catch (Exception e) {
            if (e.Message.StartsWith("Multipart body length limit") && e.Message.Contains("exceeded"))
                return BadRequest("O ficheiro introduzido é demasiado grande.");
            return BadRequest("Ocorreu um erro ao gravar o ficheiro.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> FileDelete(int fileid) {
        if (!User.Identity?.IsAuthenticated ?? false)
            return BadRequest("Not authenticated");
        var file = await _dbContext.Files.WithNoLockFirstOrDefaultAsync(i => i.Id == fileid);
        if (file != null) {
            _dbContext.Remove(file);
            await _dbContext.SaveChangesAsync();
            return Ok(true);
        }
        return NotFound();
    }
}