using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using HelpDesk.Services.Interfaces;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Provides functionality to save uploaded image files to the server.
/// </summary>
public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env)
    {
        if (string.IsNullOrWhiteSpace(env.WebRootPath))
            throw new InvalidOperationException("WebRootPath is null. Ensure 'wwwroot' folder exists.");

        _env = env;
    }

    /// <summary>
    /// Saves an uploaded image file to the specified folder and returns its relative URL path.
    /// </summary>
    /// <param name="file">The uploaded image file.</param>
    /// <param name="folderName">The folder under wwwroot to save the image (default: "uploads/projects").</param>
    /// <returns>The relative URL path of the saved image, or <c>null</c> if the file is empty.</returns>
    public async Task<string?> SaveImageFileAsync(IFormFile file, string folderName = "uploads/projects")
    {
        if (file == null || file.Length == 0)
            return null;

        string extension = Path.GetExtension(file.FileName);
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string relativePath = Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
        string absolutePath = Path.Combine(_env.WebRootPath, folderName, uniqueFileName);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        using FileStream? stream = new(absolutePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return "/" + relativePath;
    }

}