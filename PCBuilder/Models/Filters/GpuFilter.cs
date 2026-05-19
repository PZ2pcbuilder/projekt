namespace PCBuilder.Models.Filters
{
    public class GpuFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinMemory { get; set; }
        public int? MaxMemory { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? MinBoost { get; set; }
        public int? MaxBoost { get; set; }
        public int? MinPsu { get; set; }
        public int? MaxPsu { get; set; }
        public string? Chipset { get; set; }
        public string? Color { get; set; }
    }

    public class GpuIndexViewModel
    {
        public List<Gpu> Items { get; set; } = new();
        public GpuFilter Filter { get; set; } = new();
        public List<string> Chipsets { get; set; } = new();
        public List<string> Colors { get; set; } = new();
    }
}
