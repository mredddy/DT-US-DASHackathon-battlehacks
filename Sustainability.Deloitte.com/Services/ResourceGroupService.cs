using Sustainability.Deloitte.com.Controllers;
using Sustainability.Deloitte.com.Helpers;
using Sustainability.Deloitte.com.Model;

namespace Sustainability.Deloitte.com.Services
{
    public class ResourceGroupService : IResourceGroupService
    {
        private readonly IAuthentication _authentication;

        public ResourceGroupService(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        public async Task<List<ResourceGroupDTO>> GetResourceGroups()
        {
            //var token = await _authentication.GetAccessToken();
            var resources = await _authentication.GetResourceDetails();
            return resources;
        }
    }
}
