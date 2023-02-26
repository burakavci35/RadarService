using RadarService.WebApp.Areas.Authorization.Dtos;
using RadarService.WebApp.Dtos;

namespace RadarService.WebApp.Areas.Authorization.Dtos
{
    public class UserRoleDto
    {
        public string UserId { get; set; } = null!;
        public string? RoleId { get; set; }
        public UserDto? User { get; set; }
        public RoleDto? Role { get; set; }
    }
}
