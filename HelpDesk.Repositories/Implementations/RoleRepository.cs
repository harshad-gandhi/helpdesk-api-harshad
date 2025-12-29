using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository implementation for managing role-related database operations.
/// </summary>
public class RoleRepository(IDbConnectionFactory connectionFactory) : IRoleRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Retrieves all roles to populate dropdown lists in the UI.
    /// </summary>
    /// <returns>A list of <see cref="DropdownDTO"/> representing available roles.</returns>
    public async Task<List<DropdownDTO>> GetRolesAsync()
    {
        const string spName = "usp_roles_get";

        IEnumerable<DropdownDTO>? result = await _baseRepository.QueryAsync<DropdownDTO>(
            spName,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

}
