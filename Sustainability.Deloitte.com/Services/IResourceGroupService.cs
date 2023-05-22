using Sustainability.Deloitte.com.Model;

namespace Sustainability.Deloitte.com.Services
{
    public interface IResourceGroupService
    {
        Task<List<ResourceGroupDTO>> GetResourceGroups();
    }
}
