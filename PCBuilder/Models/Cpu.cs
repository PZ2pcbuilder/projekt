using System.ComponentModel.DataAnnotations;

namespace PCBuilder.Models
{
    public class Cpu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public decimal? Price { get; set; }

        public int CoreCount { get; set; }

        // Przechowujemy jako string, bo w CSV mogą być wartości typu "4.7" lub "3.8 GHz"
        public required string CoreClock { get; set; }

        public required string BoostClock { get; set; }

        public required string Microarchitecture { get; set; }

        public int? Tdp { get; set; } // Thermal Design Power w Watach

        public required string Graphics { get; set; } // Zintegrowana karta graficzna (może być puste)

        // KLUCZ KOMPATYBILNOŚCI 1: Musi pasować do Motherboard.Socket
        [Required]
        public required string Socket { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 2: Musi pasować do Motherboard.MemoryType i Memory.MemoryType
        [Required]
        public required string MemoryType { get; set; } // Np. DDR4 lub DDR5
    }
}