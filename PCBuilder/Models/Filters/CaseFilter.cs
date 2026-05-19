namespace PCBuilder.Models.Filters
{
    public class CaseFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinVolume { get; set; }
        public double? MaxVolume { get; set; }
        public double? MinMaxGpuLength { get; set; }
        public double? MaxMaxGpuLength { get; set; }
        public double? MinMaxCoolerHeight { get; set; }
        public double? MaxMaxCoolerHeight { get; set; }
        public string? Type { get; set; }
        public string? Color { get; set; }
        public string? PsuFormFactor { get; set; }
    }

    public class CaseIndexViewModel
    {
        public List<Case> Items { get; set; } = new();
        public CaseFilter Filter { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public List<string> PsuFormFactors { get; set; } = new();
    }
}
