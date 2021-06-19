using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace JurisprudenceGrabber
{
    public class SentenceGrabber
    {
        static void Main(string[] args)
        {
            var sentences = GetSentences("Emerytura a co za tym idzie swobodna ocena dowodów");
        }

        public static IList<Jurisprudence> GetSentences(string text)
        {
            // Pobieranie listy słów kluczowych z SAOS
            var keywords = GetKeywords();
            // Wyszukanie słów kluczowych we wiadomości
            var found = FindKeywordsInText(text, keywords);
            // Pobranie orzeczeń spełniających kryterium
            return GetJurisprudences(found);
        }

        public static List<Jurisprudence> GetJurisprudences(IList<string> keywords, CourtType court = CourtType.COMMON, int size = 100)
        {
            var filter = string.Empty;
            var phrases = keywords.Where(x => !string.IsNullOrWhiteSpace(x));
            if (keywords.Count > 0)
            {
                filter = $"&keywords={string.Join("&keywords=", phrases.ToArray())}";
            }

            var jurisprudences = new List<Jurisprudence>();

            Console.WriteLine("Pobieranie orzecznictwa!");

            using var client = new HttpClient {BaseAddress = new Uri("https://www.saos.org.pl/")};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.GetAsync($"api/search/judgments?pageSize={size}{filter}&courtType={court}&&sortingField=JUDGMENT_DATE&sortingDirection=DESC").Result;
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

            return jurisprudences;
        }

        public static IList<string> GetKeywords()
        {
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var keywords = new List<string>();

            Console.WriteLine("Pobieranie słów kluczowych!");

            using var client = new HttpClient {BaseAddress = new Uri("https://www.saos.org.pl/")};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var letter in alphabet)
            {
                var response = client.GetAsync($"/keywords/COMMON/{letter}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var phrases = JArray.Parse(result);
                    foreach (var phrase in phrases.Children())
                    {
                        keywords.Add(phrase["phrase"]?.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int) response.StatusCode, response.ReasonPhrase);
                }
            }

            Console.WriteLine($"Pobrano {keywords.Count} słów kluczowych");

            return ProcessKeywords(keywords);
        }

        public static IList<string> ProcessKeywords(IList<string> keywords)
        {
            Console.WriteLine("Przetwarzanie słów kluczowych!");

            keywords = keywords.Select(x => x.Replace("\\", "")).Distinct().ToList();

            var content = string.Join("\"\n\"", keywords);
            var name = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments,
                    Environment.SpecialFolderOption.None), "keywords.csv");
            //File.WriteAllText(name, $"\"{content}\"");

            Console.WriteLine($"Przetworzono {keywords.Count} słów kluczowych");

            return keywords;
        }

        public static IList<string> FindKeywordsInText(string text, IList<string> keywords)
        {
            var found = new List<string>();
            text = Regex.Replace($" {StripPunctuation(text).ToLower()} ", @"\s+", " ", RegexOptions.Multiline);
            //var stopWords = new HashSet<string>(text.Split(), StringComparer.OrdinalIgnoreCase);
            foreach (var keyword in keywords)
            {
                if (text.Contains($" {keyword} "))
                {
                    found.Add(keyword);
                }
            }

            return found;
        }

        private static string StripPunctuation(string text)
        {
            var builder = new StringBuilder();

            foreach (char character in text)
            {
                builder.Append(!char.IsPunctuation(character) ? character : ' ');
            }

            return builder.ToString();
        }
    }

    public class Jurisprudence
    {
        public Jurisprudence(string link, string court, string type, string description,
            IEnumerable<string> keywords)
        {
            Link = new Uri(link);
            Court = (CourtType)Enum.Parse(typeof(CourtType), court);
            Type = (JudgmentType)Enum.Parse(typeof(JudgmentType), type);
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

    public static class Extensions
    {
        public static string GetString(this CourtType type)
        {
            switch (type)
            {
                case CourtType.COMMON:
                    return "Sąd Powszechny";
                case CourtType.SUPREME:
                    return "Sąd Najwyższy";
                case CourtType.ADMINISTRATIVE:
                    return "Sąd Administracyjny";
                case CourtType.CONSTITUTIONAL_TRIBUNAL:
                    return "Trybunał Konstytucyjny";
                case CourtType.NATIONAL_APPEAL_CHAMBER:
                    return "Krajowa Izba Odwoławcza";
                default:
                    return "Nie zidentyfikowano";
            }
        }

        public static string GetString(this JudgmentType type)
        {
            switch (type)
            {
                case JudgmentType.DECISION:
                    return "Postanowienie";
                case JudgmentType.RESOLUTION:
                    return "Uchwała";
                case JudgmentType.SENTENCE:
                    return "Wyrok";
                case JudgmentType.REGULATION:
                    return "Zarządzenie";
                case JudgmentType.REASONS:
                    return "Uzasadnienie";
                default:
                    return "Nie zidentyfikowano";
            }
        }
    }
}
