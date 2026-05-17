using System.ComponentModel.DataAnnotations;

namespace PCBuilder.Models
{
    public class CpuCooler
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public decimal? Price { get; set; }

        // Prędkość obrotowa, często podawana jako zakres "600,2000"
        public string? Rpm { get; set; }

        // Poziom hałasu, również może być zakresem lub pojedynczą wartością
        public string? NoiseLevel { get; set; }

        public string? Color { get; set; }

        // Rozmiar chłodnicy (dla chłodzeń wodnych AIO), np. 240.0 lub 360.0
        public double? Size { get; set; }

        // Lista wspieranych gniazd, np. "LGA1700, AM4, AM5"
        // Kluczowe do sprawdzania kompatybilności z modelem Cpu
        public required string SupportedSockets { get; set; }

        // Wysokość chłodzenia w mm - kluczowe do sprawdzenia, czy zmieści się w obudowie
        // Porównujemy to z Case.MaxCpuCoolerHeightMm
        public required int HeightMm { get; set; }
    }
}
