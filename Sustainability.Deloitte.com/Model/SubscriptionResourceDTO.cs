namespace Sustainability.Deloitte.com.Model
{
    public class ResourceGroupDTO
    {
        public string ResourceGroup { get; set; }
        public List<ResourceDTO> Resources { get; set; }
    }

    public class ResourceDTO
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public string? Tier { get; set; }
        public string Type { get; set; }
        public string Namespace { get; set; }
        public decimal? CarbonEmission { get; set; }

    }
}
