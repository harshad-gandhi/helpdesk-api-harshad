namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class RolesResponse
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? ParentRoleId { get; set; }
    }
}