namespace PCBuilder.Models.Filters
{
    public class CpuFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinCores { get; set; }
        public int? MaxCores { get; set; }
        public int? MinTdp { get; set; }
        public int? MaxTdp { get; set; }
        public string? Socket { get; set; }
        public string? MemoryType { get; set; }
        public string? Microarchitecture { get; set; }
    }

    public class CpuIndexViewModel
    {
        public List<Cpu> Items { get; set; } = new();
        public CpuFilter Filter { get; set; } = new();
        public List<string> Sockets { get; set; } = new();
        public List<string> MemoryTypes { get; set; } = new();
        public List<string> Microarchitectures { get; set; } = new();
    }
}
