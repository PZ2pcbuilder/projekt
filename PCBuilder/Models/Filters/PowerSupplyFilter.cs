namespace PCBuilder.Models.Filters
{
    public class PowerSupplyFilter
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinWattage { get; set; }
        public int? MaxWattage { get; set; }
        public int? MinPcie { get; set; }
        public int? MaxPcie { get; set; }
        public string? Type { get; set; }
        public string? Efficiency { get; set; }
        public string? Modular { get; set; }
        public string? Color { get; set; }
        public string? FormFactor { get; set; }
    }

    public class PowerSupplyIndexViewModel
    {
        public List<PowerSupply> Items { get; set; } = new();
        public PowerSupplyFilter Filter { get; set; } = new();
        public List<string> Types { get; set; } = new();
        public List<string> Efficiencies { get; set; } = new();
        public List<string> Modulars { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public List<string> FormFactors { get; set; } = new();
    }
}
