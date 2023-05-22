using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Graph;
using Azure.Identity;
using Microsoft.Identity.Client;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using Azure;
using Sustainability.Deloitte.com.Model;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Sql;
using Azure.ResourceManager.AppService;
using Microsoft.Graph.Models.Security;
using System.Linq;
using Microsoft.Graph.Models;
using Azure.Core;
using System.Security.AccessControl;
using Azure.ResourceManager.Resources.Models;

namespace Sustainability.Deloitte.com.Helpers
{
    public class Authentication : IAuthentication
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public Authentication(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<string> GetAccessToken()
        {
            try
            {
                var app = ConfidentialClientApplicationBuilder.Create("e2ff0646-199a-4b78-b7db-d3f898298edf")
                 .WithClientSecret("")
                 .WithTenantId("36da45f1-dd2c-4d1f-af13-5abe46b99921")
                 .Build();

                return app.AcquireTokenForClient(new[] { "https://management.azure.com/.default" })
                 .ExecuteAsync()
                 .Result.AccessToken;

                //var keyValuePairs = new Dictionary<string, string>();
                //keyValuePairs.Add("client_id", "e2ff0646-199a-4b78-b7db-d3f898298edf");
                //keyValuePairs.Add("client_secret", "hVi8Q~PICrQ6SwfplgzwBH1UDv4TgPUbKqkGuaXG");
                //keyValuePairs.Add("scope", "https://management.azure.com/.default");
                //keyValuePairs.Add("grant_type", "client_credentials");

                //var postData = keyValuePairs.AsEnumerable();
                //using (var httpclient = new HttpClient())
                //{
                //    using (var content = new FormUrlEncodedContent(postData))
                //    {
                //        content.Headers.Clear();
                //        content.Headers.Add("content-type", "application/x-www-form-urlencoded");
                //        try
                //        {
                //            var response = await httpclient.PostAsync("https://login.microsoftonline.com/36da45f1-dd2c-4d1f-af13-5abe46b99921/oauth2/v2.0/token", content);
                //            var json = await response.Content.ReadAsStringAsync();
                //            var data = (JObject)JsonConvert.DeserializeObject(json);
                //            return data["access_token"].Value<string>();
                //        }
                //        catch (Exception ex)
                //        {
                //            throw ex;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ResourceGroupDTO>> GetResourceDetails()
        {
            var lstExclude = new List<string>() { "Microsoft.Web/sites", "microsoft.insights/actiongroups",
            "microsoft.alertsmanagement/smartDetectorAlertRules","microsoft.insights/components",
            "Microsoft.Network/publicIPAddresses","Microsoft.DevTestLab/schedules","Microsoft.Compute/virtualMachines/extensions",
                "Microsoft.Compute/virtualMachines"};

            List<ResourceGroupDTO> resourceGroupDetails = new List<ResourceGroupDTO>();
            var tenantId = "36da45f1-dd2c-4d1f-af13-5abe46b99921";
            var clientId = "e2ff0646-199a-4b78-b7db-d3f898298edf";
            var clientSecret = "hVi8Q~PICrQ6SwfplgzwBH1UDv4TgPUbKqkGuaXG";
            var scopes = new[] { "https://management.azure.com/.default" };

            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
             tenantId, clientId, clientSecret, options);

            var client = new ArmClient(clientSecretCredential, "b5973434-5104-4af6-a91d-c650c7230965");
            SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();
            ResourceGroupCollection resourceGroups = subscription.GetResourceGroups();
            var carbonEmissions = GetCarbonEmission();

            foreach (var resourceGroup1 in resourceGroups)
            {
                var resourcesGroupDTO = new ResourceGroupDTO();
                resourcesGroupDTO.ResourceGroup = resourceGroup1.Data.Name;
                ResourceGroupResource resourceGroup = await resourceGroups.GetAsync(resourceGroup1.Data.Name);
                var resourcesListDTO = new List<ResourceDTO>();

                var resourcesList = resourceGroup1.GetGenericResources()
                          .Where(resource => !lstExclude.Any(p2 => resource.Data.ResourceType.ToString() == p2)
                         )
                         .Select(rs =>
                    new ResourceDTO()
                    {
                        Location = rs.Data.Location,
                        Name = rs.Data.Name,
                        Namespace = rs.Data.ResourceType.Namespace,
                        Type = GetResourceType(rs.Data.Kind, rs.Data.ResourceType.ToString()),//rs.Data.ResourceType.Type,
                        Tier = rs.Data?.Sku?.Tier ?? string.Empty //GetTiers(rs.Data)
                    }
                    ).ToList();
                resourcesListDTO.AddRange(resourcesList);

                await foreach (VirtualMachineResource virtualMachine in resourceGroup.GetVirtualMachines())
                {
                    resourcesListDTO.Add(new ResourceDTO()
                    {
                        Location = virtualMachine.Data.Location,
                        Name = virtualMachine.Data.Name,
                        Namespace = virtualMachine.Data.ResourceType.Namespace,
                        Type = GetResourceType(null, virtualMachine.Data.ResourceType.ToString()),
                        Tier = virtualMachine.Data.HardwareProfile?.VmSize?.ToString() ?? string.Empty
                    });
                }

                await foreach (StorageAccountResource storageAcct in resourceGroup.GetStorageAccounts())
                {
                    resourcesListDTO.Add(new ResourceDTO()
                    {
                        Location = storageAcct.Data.Location,
                        Name = storageAcct.Data.Name,
                        Namespace = storageAcct.Data.ResourceType.Namespace,
                        Type = GetResourceType(null, storageAcct.Data.ResourceType.ToString()),
                        Tier = storageAcct.Data.AccessTier?.ToString() ?? string.Empty
                    });
                }

                var resources = (from resource in resourcesListDTO
                                 join co2 in carbonEmissions on new { resourceType = resource.Type, tier = resource.Tier, loc = resource.Location }
                     equals new { resourceType = co2.Service, tier = co2.Tier, loc = co2.Region } into g
                                 from ct in g.DefaultIfEmpty()
                                 select new ResourceDTO()
                                 {
                                     Location = resource.Location,
                                     Name = resource.Name,
                                     Namespace = resource.Namespace,
                                     Type = resource.Type,
                                     Tier = resource.Tier,
                                     CarbonEmission = ct?.Carbonemission ?? 0.5m
                                 }).ToList();


                resourcesGroupDTO.Resources = resources;

                resourceGroupDetails.Add(resourcesGroupDTO);
            }

            return resourceGroupDetails;
        }

        private string GetResourceType(string? kind, string type)
        {
            string resourceType = string.Empty;
            switch (type)
            {
                case "Microsoft.DataFactory/factories":
                    resourceType = "DataFactory";
                    break;
                case "Microsoft.Web/serverFarms":
                    if (kind == "app")
                        resourceType = "AppService Plan";
                    else if (kind == "functionapp")
                        resourceType = "Function App";
                    else
                        resourceType = string.Empty;
                    break;
                case "Microsoft.Compute/virtualMachines":
                    resourceType = "VirtualMachines";
                    break;
                case "Microsoft.Sql/servers":
                    resourceType = "SQL Server";
                    break;
                case "Microsoft.Sql/servers/databases":
                    resourceType = "Databases";
                    break;
                case "Microsoft.Storage/storageAccounts":
                    resourceType = "Storage Account";
                    break;
                case "Microsoft.Network/networkSecurityGroups":
                    resourceType = "Network Security Groups";
                    break;
                case "Microsoft.Network/virtualNetworks":
                    resourceType = "Virtual Networks";
                    break;
                case "Microsoft.Network/networkInterfaces":
                    resourceType = "Network Interfaces";
                    break;
                case "Microsoft.Compute/disks":
                    resourceType = "Disks";
                    break;
                case "Microsoft.EventGrid/systemTopics":
                    resourceType = "System Topics";
                    break;
                case "Microsoft.Automation/automationAccounts":
                    resourceType = "Automation Accounts";
                    break;
                default:
                    resourceType = string.Empty;
                    break;
            }
            return resourceType;
        }

        private List<AzureResourcesCO2> GetCarbonEmission()
        {
            var rootPath = _hostingEnvironment.WebRootPath; //get the root path

            var fullPath = Path.Combine(rootPath, "AzureResources.json");
            using (StreamReader r = new StreamReader(fullPath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<AzureResourcesCO2>>(json);
            }
        }
    }
}
