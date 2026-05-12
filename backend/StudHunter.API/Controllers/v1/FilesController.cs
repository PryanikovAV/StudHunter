using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.Controllers.v1.BaseControllers;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/files")]
[Authorize]
public class FilesController(IFileService fileService) : BaseController
{
    [HttpPost("images")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string type = "avatars")
    {
        var allowedTypes = new[] { "avatars", "banners", "logos" };
        if (!allowedTypes.Contains(type.ToLower()))
            return BadRequest(new { message = "Недопустимый тип изображения." });

        try
        {
            var url = await fileService.SaveImageAsync(file, type.ToLower());
            return Ok(new { Url = url });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}