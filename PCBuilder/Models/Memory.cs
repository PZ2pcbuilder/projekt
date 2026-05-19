using System.ComponentModel.DataAnnotations;

namespace PCBuilder.Models{
    public class Memory {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        public required string Speed { get; set; }
        public required string Modules { get; set; }
        public required int Capacity { get; set; }
        public decimal? PricePerGb { get; set; }
        public required string Color { get; set; }
        public double? FirstWordLatency { get; set; }
        public double? CasLatency { get; set; }
        public required string MemoryType { get; set; } 
    }
}
