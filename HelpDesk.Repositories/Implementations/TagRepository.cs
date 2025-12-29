using System.Data;
using System.Text.Json;
using Dapper;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class TagRepository(IDbConnectionFactory connectionFactory) : ITagRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    public async Task<TagCreateResponseDTO?> CreateTagAsync(TagCreateRequestDTO request)
    {
        const string spName = "usp_tag_create";

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", request.ProjectId);
        parameters.Add("@Name", request.Name);
        parameters.Add("@CreatedBy", request.CreatedBy);

        TagCreateResponseDTO? result = await _baseRepository.QueryFirstOrDefaultAsync<TagCreateResponseDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<IEnumerable<TagResponseDTO>> GetTagsByProjectAndSearchAsync(TagGetRequestDto request)
    {
        const string spName = "usp_tag_get";

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", request.ProjectId);
        parameters.Add("@Search", request.Search ?? string.Empty);
        Console.WriteLine(request.Search + "search");

        IEnumerable<TagResponseDTO> tags = await _baseRepository.QueryAsync<TagResponseDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return tags;
    }
}
