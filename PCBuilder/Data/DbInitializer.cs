using PCBuilder.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;


namespace PCBuilder.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            
            context.Database.EnsureCreated();

            Console.WriteLine(">>> Baza danych została stworzona na nowo.");

            // Teraz importowanie danych...
            string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data");
            if (context.Cpus.Any()) return;


            // --- 1. CPU ---
            if (File.Exists(Path.Combine(dataPath, "cpu.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "cpu.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.Cpus.Add(new Cpu {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        CoreCount = int.Parse(p[2]),
                        CoreClock = p[3],
                        BoostClock = p[4],
                        Microarchitecture = p[5],
                        Tdp = int.TryParse(p[6], out int tdp) ? tdp : 0,
                        Graphics = p[7],
                        Socket = p[8],
                        MemoryType = p[9]
                    });
                }
            }

            // --- 2. MEMORY (Dopasowane do Twojego CSV: name,price,speed,modules,price_per_gb,color,lat1,lat2,type) ---
            if (File.Exists(Path.Combine(dataPath, "memory.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "memory.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.Memories.Add(new Memory {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        Speed = int.Parse(p[2]),
                        Color = p[3],
                        MemoryType = p[4], // 8. kolumna (indeks 7)
                        Capacity = int.Parse(p[5]),
                        Modules = p[6],
                    });
                }
            }

            // --- 3. GPU ---
            if (File.Exists(Path.Combine(dataPath, "video-card.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "video-card.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.Gpus.Add(new Gpu {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        Chipset = p[2],
                        Memory = int.Parse(p[3]),
                        CoreClock = int.Parse(p[4]),
                        BoostClock = int.Parse(p[5]),
                        Color = p[6],
                        Length = int.Parse(p[7]),
                        RecommendedPsuW = int.TryParse(p[8], out int rpsu) ? rpsu : 0,
                        PowerConnectors = p[9]
                    });
                }
            }

            // --- 4. MOTHERBOARD ---
            if (File.Exists(Path.Combine(dataPath, "motherboard.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "motherboard.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.Motherboards.Add(new Motherboard {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        Socket = p[2],
                        FormFactor = p[3],
                        MaxMemory = int.TryParse(p[4], out int mm) ? mm : -1,
                        MemorySlots = int.TryParse(p[5], out int ms) ? ms : -1,
                        Color = p[6],
                        MemoryType = p[7],
                        M2Slots = int.TryParse(p[8], out int m2) ? m2 : -1,
                        SataPorts = int.TryParse(p[9], out int sata) ? sata : -1
                    });
                }
            }

            // --- 5. CASE ---
            if (File.Exists(Path.Combine(dataPath, "case.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "case.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.Cases.Add(new Case {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        Type = p[2],
                        Color = p[3],
                        SidePanel = p[4],
                        ExternalVolume = ParseDouble(p[5]),
                        SupportedMoboFormFactors = p[6],
                        MaxGpuLengthMm = ParseDouble(p[7]),
                        MaxCpuCoolerHeightMm = ParseDouble(p[8]),
                        PsuFormFactor = p[9]
                    });
                }
            }

            // --- 6. POWER SUPPLY ---
            if (File.Exists(Path.Combine(dataPath, "power-supply.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "power-supply.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.PowerSupplies.Add(new PowerSupply {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        Type = p[2],
                        Efficiency = p[3],
                        Wattage = int.TryParse(p[4], out int watt) ? watt : 0,
                        Modular = p[5],
                        Color = p[6],
                        FormFactor = p[7],
                        Pcie6Plus2Connectors = int.TryParse(p[8], out int conn) ? conn : -1
                    });
                }
            }

            // --- 7. STORAGE ---
            if (File.Exists(Path.Combine(dataPath, "internal-hard-drive.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "internal-hard-drive.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.Storages.Add(new Storage {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        Capacity = int.Parse(p[2]),
                        Type = p[3],
                        Cache = int.Parse(p[4]),
                        FormFactor = p[5],
                        Interface = p[6]
                    });
                }
            }

            // --- 8. CPU COOLER ---
            if (File.Exists(Path.Combine(dataPath, "cpu-cooler.csv")))
            {
                var lines = File.ReadAllLines(Path.Combine(dataPath, "cpu-cooler.csv")).Skip(1);
                foreach (var line in lines)
                {
                    var p = SplitCsv(line);
                    context.CpuCoolers.Add(new CpuCooler {
                        Name = p[0],
                        Price = ParseDecimal(p[1]),
                        Rpm = int.Parse(p[2]),
                        NoiseLevel = ParseDouble(p[3]),
                        Color = p[4],
                        Size = int.Parse(p[5]),
                        SupportedSockets = p[6],
                        HeightMm = int.TryParse(p[7], out int height) ? height : -1
                    });
                }
            }

            // Users
            if (!context.Users.Any(u => u.Username == "admin"))
                {
                    var admin = new User
                    {
                        Username = "admin",
                        Role = "Admin",
                        ApiToken = Guid.NewGuid().ToString(),
                        PasswordHash = new PasswordHasher<User>().HashPassword(null!, "admin")
                    };
                    
                    context.Users.Add(admin);
                }

            context.SaveChanges();
        }

        private static string[] SplitCsv(string line)
        {
            return Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        }

        private static decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.ToLower() == "null") return null;
            value = value.Replace("\"", "").Trim();
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal res)) return res;
            return null;
        }

        private static double? ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.ToLower() == "null") return null;
            value = value.Replace("\"", "").Trim();
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double res)) return res;
            return null;
        }
    }
}