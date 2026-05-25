using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PCBuilder.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string apiUrl = "https://localhost:5288/api/data/cpus"; 
            
            using HttpClient client = new HttpClient();
            
            // AUTORYZACJA REST API (zgodna z Twoim poleceniem)
            client.DefaultRequestHeaders.Add("X-username", "oski");
            client.DefaultRequestHeaders.Add("X-Api-Token", "60b7a11c026448efb17a1236fc900851"); 
            
            Console.WriteLine("=== KLIENT KONSOLOWY PC BUILDER REST API ===");
            Console.WriteLine("Wysyłam zapytanie do serwera...");
            
            try 
            {
                var response = await client.GetAsync(apiUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("\nSUKCES! Oto pobrane dane z bazy:");
                    Console.WriteLine(data);
                }
                else
                {
                    Console.WriteLine($"\nBłąd serwera lub brak autoryzacji. Kod błędu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nBłąd połączenia. Upewnij się, że aplikacja webowa jest uruchomiona. Szczegóły: {ex.Message}");
            }

            Console.WriteLine("\nNaciśnij Enter, aby zakończyć...");
            Console.ReadLine();
        }
    }
}