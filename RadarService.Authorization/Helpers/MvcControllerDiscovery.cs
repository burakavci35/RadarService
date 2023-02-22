
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RadarService.Authorization.Dtos;
using System.ComponentModel;
using System.Reflection;

namespace RadarService.Authorization.Helpers
{
    public class MvcControllerDiscovery : IMvcControllerDiscovery
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public MvcControllerDiscovery(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public IEnumerable<MvcControllerInfo> GetControllerInfos()
        {
            var mvcControllerInfos = _actionDescriptorCollectionProvider
                 .ActionDescriptors.Items
                 .Where(descriptor => descriptor.GetType() == typeof(ControllerActionDescriptor))
                 .Select(descriptor => (ControllerActionDescriptor)descriptor)
                 .GroupBy(descriptor => descriptor.ControllerTypeInfo.FullName)
                 .Select(descriptor => new MvcControllerInfo()
                 {
                     AreaName = string.IsNullOrEmpty(descriptor.First().ControllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue) ? "Default" : descriptor.First().ControllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                     DisplayName = descriptor.First().ControllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                     Name = descriptor.First().ControllerName,
                 })
                 .ToList();

            return mvcControllerInfos;
        }
    }
}
