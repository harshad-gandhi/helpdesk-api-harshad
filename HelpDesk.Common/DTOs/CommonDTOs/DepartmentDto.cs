namespace HelpDesk.Common.DTOs.CommonDTOs;

public class DepartmentDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string DepartmentMembers { get; set; }
    public int ActiveChatsCount { get; set; }
    public int CreatedBy {get;set;}
    public int UpdatedBy {get;set;}
}
