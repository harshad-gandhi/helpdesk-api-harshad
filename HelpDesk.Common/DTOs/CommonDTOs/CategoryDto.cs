namespace HelpDesk.Common.DTOs.CommonDTOs;

public class CategoryDto
{
    public int? Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public string? IconUrl { get; set; }= string.Empty;
    public int? ParentCategoryId { get; set; } = null;
    public int CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
