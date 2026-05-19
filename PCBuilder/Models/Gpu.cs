using System.ComponentModel.DataAnnotations;


namespace PCBuilder.Models
{
    public class Gpu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public decimal? Price { get; set; }

        public required string Chipset { get; set; } // np. GeForce RTX 4070

        // Pamięć VRAM w GB (np. 12.0)
        public int? Memory { get; set; }

        public int? CoreClock { get; set; } // w MHz

        public int? BoostClock { get; set; } // w MHz

        public string? Color { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 1: Długość karty w mm
        // Sprawdzamy, czy Gpu.Length <= Case.MaxGpuLengthMm
        public int? Length { get; set; }


        public required int RecommendedPsuW { get; set; }

        public required string PowerConnectors { get; set; } // np. "1x 12VHPWR" lub "2x 8-pin"
    }
}
