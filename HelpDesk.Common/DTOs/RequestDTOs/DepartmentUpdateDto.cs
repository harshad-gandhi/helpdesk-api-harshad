namespace HelpDesk.Common.DTOs.RequestDTOs;

public class DepartmentUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int UpdatedBy { get; set; }
}
