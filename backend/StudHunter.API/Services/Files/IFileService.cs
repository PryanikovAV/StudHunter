public interface IFileService
{
    Task<string> SaveImageAsync(IFormFile file, string folderName);
    void DeleteImage(string relativePath);
}