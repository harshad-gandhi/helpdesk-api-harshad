using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Repositories.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HelpDesk.Repositories.Implementations;

public class ChatAttachmentsRepository(IDbConnectionFactory connectionFactory) : IChatAttachmentsRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    public async Task<StoredProcedureResult<long>> CreateChatAttachmentAsync(ChatAttachmentDTO chatAttachmentDTO)
    {
        const string spName = "usp_chat_attachments_save";

        DynamicParameters parameters = new();

        parameters.Add("@Id", chatAttachmentDTO.Id);
        parameters.Add("@ChatMessageId", chatAttachmentDTO.ChatMessageId);
        parameters.Add("@FileName", chatAttachmentDTO.FileName);
        parameters.Add("@OriginalFileName", chatAttachmentDTO.OriginalFileName);
        parameters.Add("@FilePath", chatAttachmentDTO.FilePath);
        parameters.Add("@MimeType", chatAttachmentDTO.MimeType);
        parameters.Add("@FileSizeByte", chatAttachmentDTO.FileSizeByte);
        parameters.Add("@IsDeleted", chatAttachmentDTO.IsDeleted);
        parameters.Add("@ResultId", dbType: DbType.Int64, direction: ParameterDirection.Output);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new StoredProcedureResult<long>
        {
            Data = parameters.Get<long>("ResultId"),
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }
}
