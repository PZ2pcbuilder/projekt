namespace PCBuilder.Models.Filters
{
    public class MemoryFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public int? MinSpeed { get; set; }
        public int? MaxSpeed { get; set; }
        public string? MemoryType { get; set; }
        public string? Color { get; set; }
        public string? Modules { get; set; }
    }

    public class MemoryIndexViewModel
    {
        public List<Memory> Items { get; set; } = new();
        public MemoryFilter Filter { get; set; } = new();
        public List<string> MemoryTypes { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public List<string> ModulesList { get; set; } = new();
    }
}
