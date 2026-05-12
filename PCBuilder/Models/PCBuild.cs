namespace PCBuilder.Models
{
    public class PcBuild
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Relacje do wszystkich 8 komponentów
    public int? CpuId { get; set; }
    public required Cpu Cpu { get; set; }

    public int? MotherboardId { get; set; }
    public required Motherboard Motherboard { get; set; }

    public int? MemoryId { get; set; }
    public required Memory Memory { get; set; }

    public int? GpuId { get; set; }
    public required Gpu Gpu { get; set; }

    public int? CaseId { get; set; }
    public required Case Case { get; set; }

    public int? CpuCoolerId { get; set; }
    public required CpuCooler CpuCooler { get; set; }

    public int? StorageId { get; set; }
    public required Storage Storage { get; set; }

    public int? PowerSupplyId { get; set; }
    public required PowerSupply PowerSupply { get; set; }

    // Relacja do użytkownika
    public int UserId { get; set; }
    public required User User { get; set; }
}
}