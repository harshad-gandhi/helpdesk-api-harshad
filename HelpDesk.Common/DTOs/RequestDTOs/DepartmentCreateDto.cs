namespace HelpDesk.Common.DTOs.RequestDTOs;

public class DepartmentCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int CreatedBy { get; set;}
}
