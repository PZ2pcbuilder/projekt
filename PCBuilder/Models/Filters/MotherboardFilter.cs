namespace PCBuilder.Models.Filters
{
    public class MotherboardFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinMemorySlots { get; set; }
        public int? MaxMemorySlots { get; set; }
        public int? MinMaxMemory { get; set; }
        public int? MaxMaxMemory { get; set; }
        public int? MinM2Slots { get; set; }
        public int? MaxM2Slots { get; set; }
        public int? MinSataPorts { get; set; }
        public int? MaxSataPorts { get; set; }
        public string? Socket { get; set; }
        public string? FormFactor { get; set; }
        public string? MemoryType { get; set; }
        public string? Color { get; set; }
    }

    public class MotherboardIndexViewModel
    {
        public List<Motherboard> Items { get; set; } = new();
        public MotherboardFilter Filter { get; set; } = new();
        public List<string> Sockets { get; set; } = new();
        public List<string> FormFactors { get; set; } = new();
        public List<string> MemoryTypes { get; set; } = new();
        public List<string> Colors { get; set; } = new();
    }
}
