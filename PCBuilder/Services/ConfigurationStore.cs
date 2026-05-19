using System.Text.Json;
using Microsoft.AspNetCore.Http;
using PCBuilder.Data;

namespace PCBuilder.Services
{
    /// <summary>
    /// Pomocnik do zapisu/odczytu wyboru komponentów (sesja <-> kolumna Users.SavedConfigJson).
    /// </summary>
    public static class ConfigurationStore
    {
        // Pojedyncze źródło prawdy o nazwach kluczy sesji
        public static readonly string[] ComponentKeys = new[]
        {
            "SelectedCpuId",
            "SelectedGpuId",
            "SelectedMotherboardId",
            "SelectedMemoryId",
            "SelectedCaseId",
            "SelectedCpuCoolerId",
            "SelectedStorageId",
            "SelectedPsuId"
        };

        public static string SerializeSession(ISession session)
        {
            var dict = new Dictionary<string, int>();
            foreach (var key in ComponentKeys)
            {
                var v = session.GetInt32(key);
                if (v.HasValue) dict[key] = v.Value;
            }
            return JsonSerializer.Serialize(dict);
        }

        public static void RestoreToSession(ISession session, string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                if (dict == null) return;
                foreach (var key in ComponentKeys)
                {
                    if (dict.TryGetValue(key, out var v))
                        session.SetInt32(key, v);
                    else
                        session.Remove(key);
                }
            }
            catch
            {
                // ignorujemy uszkodzony JSON
            }
        }

        public static void PersistSessionToUser(ISession session, ApplicationDbContext db, int userId)
        {
            var user = db.Users.Find(userId);
            if (user == null) return;
            user.SavedConfigJson = SerializeSession(session);
            db.SaveChanges();
        }
    }
}
