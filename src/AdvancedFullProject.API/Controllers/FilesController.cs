using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdvancedFullProject.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/files")]
[Authorize]
public sealed class FilesController : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploads);
        var filePath = Path.Combine(uploads, file.FileName);
        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream, cancellationToken);
        return Ok(new { file.FileName, file.Length });
    }

    [HttpGet("stream")]
    [AllowAnonymous]
    public async Task Stream(CancellationToken cancellationToken)
    {
        Response.ContentType = "text/plain";
        for (var i = 1; i <= 5; i++)
        {
            await Response.WriteAsync($"Chunk {i}\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
            await Task.Delay(300, cancellationToken);
        }
    }
}
