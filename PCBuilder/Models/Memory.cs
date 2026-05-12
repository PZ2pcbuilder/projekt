using System.ComponentModel.DataAnnotations;

namespace PCBuilder.Models{
    public class Memory {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal? Price { get; set; }
        public required string Speed { get; set; }
        public required string MemoryType { get; set; } 
        public double TotalCapacity { get; set; }
        public int ModuleCount { get; set; }
    }
}