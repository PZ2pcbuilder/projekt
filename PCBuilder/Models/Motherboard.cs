using System.ComponentModel.DataAnnotations;

namespace PCBuilder.Models
{
    public class Motherboard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public decimal? Price { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 1: Musi być identyczny z Cpu.Socket
        [Required]
        public required string Socket { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 2: Musi pasować do Case.SupportedMoboFormFactors
        public required string FormFactor { get; set; } // np. ATX, Micro ATX, Mini ITX

        public required int MaxMemory { get; set; } // np. 128 lub 192 (GB)

        public required int MemorySlots { get; set; } // Zazwyczaj 2 lub 4

        public required string Color { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 3: Musi pasować do Cpu.MemoryType i Memory.MemoryType
        public required string MemoryType { get; set; } // DDR4 lub DDR5

        // Logika dodatkowa: limit dysków M.2
        public required int M2Slots { get; set; }

        // Logika dodatkowa: limit dysków SATA
        public required int SataPorts { get; set; }
    }
}
