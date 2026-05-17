using System.ComponentModel.DataAnnotations;

namespace PCBuilder.Models
{
    public class Storage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public decimal? Price { get; set; }

        // Pojemność w GB (np. 1000.0 lub 2000.0)
        public double? Capacity { get; set; }

        public decimal? PricePerGb { get; set; }

        public required string Type { get; set; } // SSD lub HDD

        // Cache w MB (może być puste dla niektórych dysków)
        public double? Cache { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 1: np. "M.2-2280" lub "2.5\""
        // Pozwala sprawdzić, czy płyta ma odpowiednie złącza
        public required string FormFactor { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 2: np. "SATA 6.0 Gb/s" lub "M.2 PCIe 4.0 X4"
        public required string Interface { get; set; }
    }
}
