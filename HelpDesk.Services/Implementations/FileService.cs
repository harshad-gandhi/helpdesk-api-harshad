using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using static HelpDesk.Common.Enums.Enumerations;

namespace HelpDesk.Services.Implementations;

public class FileService(IStringLocalizer<Messages> localizer, IWebHostEnvironment env) : IFileService
{
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    private readonly IWebHostEnvironment _env = env;

    public async Task<TicketEventAttachmentDto> GetFileDetails(IFormFile file)
    {
        if (file == null)
        {
            throw new FileNullException(_localizer["FILE_NULL"]);
        }

        var guid = Guid.NewGuid().ToString("N");
        var folderName = guid.Substring(0, 2);
        var uploadsPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", folderName);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var ext = Path.GetExtension(file.FileName);
        var storedFilename = $"{guid}{ext}";
        var filePath = Path.Combine(uploadsPath, storedFilename);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        TicketEventAttachmentDto fileDetails = new TicketEventAttachmentDto
        {
            Filename = storedFilename,
            OriginalFilename = file.FileName,
            FilePath = $"/uploads/{folderName}/{storedFilename}",
            MimeType = (int)GetMimeType(file.ContentType),
            FileSizeBytes = file.Length
        };

        return fileDetails;
    }

    private MimeType GetMimeType(string contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return MimeType.Unknown;

        if (contentType.Contains("pdf")) return MimeType.Pdf;
        if (contentType.Contains("image")) return MimeType.Png;
        if (contentType.Contains("plain")) return MimeType.TextPlain;

        return MimeType.Unknown;
    }
}
