using System.ComponentModel.DataAnnotations;

namespace PCBuilder.Models
{
    public class Case
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        // Cena może być nullem w CSV, dlatego używamy decimal?
        public decimal? Price { get; set; }

        public required string Type { get; set; } // np. ATX Mid Tower

        public required string Color { get; set; }

        public string? Psu { get; set; } // Informacja o wbudowanym zasilaczu

        public string? SidePanel { get; set; }

        public double? ExternalVolume { get; set; } // Objętość w litrach

        public required int Internal35Bays { get; set; } // Liczba zatok 3.5"

        // Ważne dla konfiguratora: lista wspieranych formatów płyt (np. "ATX, Micro ATX")
        public required string SupportedMoboFormFactors { get; set; }

        public double? MaxGpuLengthMm { get; set; }

        public double? MaxCpuCoolerHeightMm { get; set; }

        public required string PsuFormFactor { get; set; } // np. ATX
    }
}