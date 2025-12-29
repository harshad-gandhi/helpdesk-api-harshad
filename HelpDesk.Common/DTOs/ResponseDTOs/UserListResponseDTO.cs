namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class UserListResponseDTO
    {
        public int UserId { get; set; }
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
    }

    public class DepartmentUserListResponseDTO
    {
        public string DepartmentName { get; set; } = string.Empty;
        public List<UserListResponseDTO> Users { get; set; } = [];
    }
}