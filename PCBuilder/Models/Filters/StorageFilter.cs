namespace PCBuilder.Models.Filters
{
    public class StorageFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public int? MinCache { get; set; }
        public int? MaxCache { get; set; }
        public string? Type { get; set; }
        public string? FormFactor { get; set; }
        public string? Interface { get; set; }
    }

    public class StorageIndexViewModel
    {
        public List<Storage> Items { get; set; } = new();
        public StorageFilter Filter { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public List<string> FormFactors { get; set; } = new();
        public List<string> Interfaces { get; set; } = new();
    }
}
