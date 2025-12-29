namespace HelpDesk.Common.DTOs.CommonDTOs;

public class DropdownDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? GroupName { get; set; }   // optional
}
