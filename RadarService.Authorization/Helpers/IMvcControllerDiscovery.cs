using RadarService.Authorization.Dtos;

namespace RadarService.Authorization.Helpers
{
    public interface IMvcControllerDiscovery
    {
        IEnumerable<MvcControllerInfo> GetControllerInfos();
    }
}
