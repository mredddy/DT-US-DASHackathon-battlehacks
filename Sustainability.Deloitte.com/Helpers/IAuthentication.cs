using Sustainability.Deloitte.com.Model;

namespace Sustainability.Deloitte.com.Helpers
{
    public interface IAuthentication
    {
        Task<string> GetAccessToken();
        Task<List<ResourceGroupDTO>> GetResourceDetails();
    }
}
