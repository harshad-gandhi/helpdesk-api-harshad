using Microsoft.AspNetCore.Http;

namespace HelpDesk.Services.Interfaces;

public interface IImageService
{
      Task<string?> SaveImageFileAsync(IFormFile file, string folderName = "uploads/projects");
}
