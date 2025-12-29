using Microsoft.AspNetCore.Http;
using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IFileService
{
    Task<TicketEventAttachmentDto> GetFileDetails(IFormFile file);
}
