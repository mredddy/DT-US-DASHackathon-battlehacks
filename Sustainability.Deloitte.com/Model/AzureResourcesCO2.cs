namespace Sustainability.Deloitte.com.Model
{
    public class AzureResourcesCO2
    {
        public string Region { get; set; }
        public string Service { get; set; }
        public string? Tier { get; set; }
        public decimal? Carbonemission { get; set; }
    }
}
