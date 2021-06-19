using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JurisprudenceGrabber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var jurisprudences = new List<Jurisprudence>();

            Console.WriteLine("Pobieranie orzecznictwa!");
            using var client = new HttpClient {BaseAddress = new Uri("https://www.saos.org.pl/")};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("api/search/judgments?pageSize=100&courtType=COMMON&sortingField=JUDGMENT_DATE&sortingDirection=DESC").Result;  // Blocking call!    
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(result);
                var items = json["items"];

                if (items != null)
                {
                    foreach (var child in items.Children())
                    {
                        jurisprudences.Add(new Jurisprudence(child["href"]?.ToString(), child["courtType"]?.ToString(), 
                            child["judgmentType"]?.ToString(), child["textContent"]?.ToString(),
                            child["keywords"]?.ToArray().Select(x => x.ToString())));
                    }
                }

                Console.WriteLine($"Pobrano {jurisprudences.Count} orzeczeń");
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.ReasonPhrase);
            }
        }
    }

    public class Jurisprudence
    {
        public Jurisprudence(string link, string court, string type, string description, IEnumerable<string> keywords)
        {
            Link = new Uri(link);
            Court = (CourtType) Enum.Parse(typeof(CourtType), court);
            Type = (JudgmentType) Enum.Parse(typeof(JudgmentType), type);
            Description = description;
            Keywords = keywords.ToList();
        }

        public Uri Link { get; }

        public CourtType Court { get; }

        public JudgmentType Type { get; }

        public string Description { get; }

        public IList<string> Keywords { get; }

    }

    public enum CourtType
    {
        COMMON,
        SUPREME,
        ADMINISTRATIVE,
        CONSTITUTIONAL_TRIBUNAL,
        NATIONAL_APPEAL_CHAMBER
    }

    public enum JudgmentType
    {
        DECISION,
        RESOLUTION,
        SENTENCE,
        REGULATION,
        REASONS
    }
}
