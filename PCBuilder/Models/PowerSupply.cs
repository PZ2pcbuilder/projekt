using System.ComponentModel.DataAnnotations;


namespace PCBuilder.Models
{
    public class PowerSupply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public decimal? Price { get; set; }

        public required string Type { get; set; } // np. ATX

        public string? Efficiency { get; set; } // np. Gold, Bronze, Platinum

        // KLUCZ LOGIKI 1: Całkowita moc zasilacza
        // Pozwala sprawdzić, czy suma TDP procesora i karty graficznej nie przekracza mocy
        public required int Wattage { get; set; }

        // Czy zasilacz jest modularny (Full, Semi, false)
        public required string Modular { get; set; }

        public string? Color { get; set; }

        // KLUCZ KOMPATYBILNOŚCI 1: Musi pasować do Case.PsuFormFactor
        public required string FormFactor { get; set; } // np. ATX, SFX

        // Liczba wtyczek do karty graficznej
        public required int Pcie6Plus2Connectors { get; set; }
    }
}
