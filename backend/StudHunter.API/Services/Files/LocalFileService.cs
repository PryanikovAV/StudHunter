namespace StudHunter.API.Services.Files;

public class LocalFileService(IWebHostEnvironment env) : IFileService
{
    public async Task<string> SaveImageAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл пуст или не передан.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Недопустимый формат файла. Разрешены только изображения.");

        if (file.Length > 5 * 1024 * 1024)  // <- Ограничения
            throw new ArgumentException("Размер файла не должен превышать 5 МБ.");

        var uploadsFolder = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", folderName);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(fileStream);

        return $"/uploads/{folderName}/{uniqueFileName}";
    }

    public void DeleteImage(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        var fileName = Path.GetFileName(relativePath);
        var filePath = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "folderName", fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
