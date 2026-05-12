using Microsoft.EntityFrameworkCore;
using PCBuilder.Models;

namespace PCBuilder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabele komponentów
        public DbSet<Cpu> Cpus { get; set; }
        public DbSet<Motherboard> Motherboards { get; set; }
        public DbSet<Memory> Memories { get; set; }
        public DbSet<Gpu> Gpus { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<CpuCooler> CpuCoolers { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<PowerSupply> PowerSupplies { get; set; }

        // Tabele systemowe
        public DbSet<User> Users { get; set; }
        public DbSet<PcBuild> PcBuilds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Konfiguracja precyzji cen dla wszystkich tabel
            var decimalEntities = new[] { 
                typeof(Cpu), typeof(Motherboard), typeof(Memory), typeof(Gpu), 
                typeof(Case), typeof(CpuCooler), typeof(Storage), typeof(PowerSupply) 
            };

            foreach (var type in decimalEntities)
            {
                modelBuilder.Entity(type).Property("Price").HasColumnType("decimal(18,2)");
            }

            // 2. Jawne zdefiniowanie relacji w PcBuild (Fluent API)
            // Dzięki temu EF stworzy poprawne klucze obce (Foreign Keys) w bazie danych
            
            modelBuilder.Entity<PcBuild>(entity =>
            {
                entity.HasOne(d => d.Cpu).WithMany().HasForeignKey(d => d.CpuId).IsRequired(false);
                entity.HasOne(d => d.Motherboard).WithMany().HasForeignKey(d => d.MotherboardId).IsRequired(false);
                entity.HasOne(d => d.Memory).WithMany().HasForeignKey(d => d.MemoryId).IsRequired(false);
                entity.HasOne(d => d.Gpu).WithMany().HasForeignKey(d => d.GpuId).IsRequired(false);
                entity.HasOne(d => d.Case).WithMany().HasForeignKey(d => d.CaseId).IsRequired(false);
                entity.HasOne(d => d.CpuCooler).WithMany().HasForeignKey(d => d.CpuCoolerId).IsRequired(false);
                entity.HasOne(d => d.Storage).WithMany().HasForeignKey(d => d.StorageId).IsRequired(false);
                entity.HasOne(d => d.PowerSupply).WithMany().HasForeignKey(d => d.PowerSupplyId).IsRequired(false);
                

                entity.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}