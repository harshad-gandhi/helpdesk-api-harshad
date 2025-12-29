using HelpDesk.Common.DTOs.CommonDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<List<DropdownDTO>> GetRolesAsync();
}
