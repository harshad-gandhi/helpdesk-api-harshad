using HelpDesk.Common.DTOs.CommonDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IChatAttachmentsRepository
{
    Task<StoredProcedureResult<long>> CreateChatAttachmentAsync(ChatAttachmentDTO chatAttachmentDTO);
}
