using Microsoft.AspNetCore.Mvc;
using Sustainability.Deloitte.com.Helpers;
using Sustainability.Deloitte.com.Services;

namespace Sustainability.Deloitte.com.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarbonEmissionController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<CarbonEmissionController> _logger;
        private readonly IResourceGroupService _resourceGroupService;

        public CarbonEmissionController(ILogger<CarbonEmissionController> logger, IResourceGroupService resourceGroupService)
        {
            _resourceGroupService = resourceGroupService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            //await _authentication.GetAccessToken();
            //Authentication authentication = new Authentication();
            var result = await _resourceGroupService.GetResourceGroups();

            return Ok(result);
        }
    }
}