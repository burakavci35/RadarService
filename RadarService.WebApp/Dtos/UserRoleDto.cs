namespace RadarService.WebApp.Dtos
{
    public class UserRoleDto
    {
        public string UserId { get; set; } = null!;
        public string? RoleId { get; set; }
        public UserDto? User { get; set; }
        public RoleDto? Role { get; set; } 
    }
}
