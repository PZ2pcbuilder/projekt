namespace PCBuilder.Models.Filters
{
    public class CpuCoolerFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinRpm { get; set; }
        public int? MaxRpm { get; set; }
        public double? MinNoise { get; set; }
        public double? MaxNoise { get; set; }
        public int? MinHeight { get; set; }
        public int? MaxHeight { get; set; }
        public int? MinSize { get; set; }
        public int? MaxSize { get; set; }
        public string? Color { get; set; }
        public string? Socket { get; set; }
    }

    public class CpuCoolerIndexViewModel
    {
        public List<CpuCooler> Items { get; set; } = new();
        public CpuCoolerFilter Filter { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public List<string> Sockets { get; set; } = new();
    }
}
