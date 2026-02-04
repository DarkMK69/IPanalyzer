using IpAnalyzer.Interfaces;
using IpAnalyzer.Models;
using Newtonsoft.Json;

namespace IpAnalyzer.Services
{
    /// <summary>
    /// Реализация издателя статистики в файлы
    /// </summary>
    public class FileIpStatisticsPublisher : IIpStatisticsPublisher
    {
        private readonly string _outputDirectory;

        public FileIpStatisticsPublisher(string outputDirectory = "output")
        {
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));
            
            // Создаем директорию, если её нет
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        /// <summary>
        /// Опубликовать данные в файлы
        /// </summary>
        public async Task PublishAsync<T>(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
            {
                Console.WriteLine("⚠️  Нет данных для выгрузки в файлы");
                return;
            }

            var firstItem = data.FirstOrDefault();

            if (firstItem is CountryIpDetails)
            {
                await ExportCountryStatisticsAsync((IEnumerable<CountryIpDetails>)(object)data);
            }
            else if (firstItem is CityIpDetails)
            {
                await ExportCityStatisticsAsync((IEnumerable<CityIpDetails>)(object)data);
            }
        }

        /// <summary>
        /// Экспортировать статистику по странам в CSV и JSON
        /// </summary>
        private async Task ExportCountryStatisticsAsync(IEnumerable<CountryIpDetails> countryStats)
        {
            var countryList = countryStats.ToList();

            // Экспорт в CSV
            await ExportCountryToCsvAsync(countryList);

            // Экспорт в JSON
            await ExportCountryToJsonAsync(countryList);
        }

        /// <summary>
        /// Экспортировать статистику по городам в CSV и JSON
        /// </summary>
        private async Task ExportCityStatisticsAsync(IEnumerable<CityIpDetails> cityStats)
        {
            var cityList = cityStats.ToList();

            // Экспорт в CSV
            await ExportCityToCsvAsync(cityList);

            // Экспорт в JSON
            await ExportCityToJsonAsync(cityList);
        }

        /// <summary>
        /// Экспортировать статистику по странам в CSV
        /// </summary>
        private async Task ExportCountryToCsvAsync(List<CountryIpDetails> countryStats)
        {
            var csvPath = Path.Combine(_outputDirectory, "countries_statistics.csv");

            var lines = new List<string>
            {
                "Код страны,Название страны,Количество IP,Процент"
            };

            foreach (var stat in countryStats)
            {
                lines.Add($"{EscapeCsvValue(stat.CountryCode)},{EscapeCsvValue(stat.CountryName)},{stat.Count},{stat.Percentage:F2}%");
            }

            await File.WriteAllLinesAsync(csvPath, lines);
            Console.WriteLine($"✓ Статистика по странам экспортирована в CSV: {csvPath}");
        }

        /// <summary>
        /// Экспортировать статистику по городам в CSV
        /// </summary>
        private async Task ExportCityToCsvAsync(List<CityIpDetails> cityStats)
        {
            var csvPath = Path.Combine(_outputDirectory, "cities_statistics.csv");

            var lines = new List<string>
            {
                "Город,Область,Код страны,Количество IP,Процент"
            };

            foreach (var stat in cityStats)
            {
                lines.Add($"{EscapeCsvValue(stat.City)},{EscapeCsvValue(stat.Region)},{EscapeCsvValue(stat.CountryCode)},{stat.Count},{stat.Percentage:F2}%");
            }

            await File.WriteAllLinesAsync(csvPath, lines);
            Console.WriteLine($"✓ Статистика по городам экспортирована в CSV: {csvPath}");
        }

        /// <summary>
        /// Экспортировать статистику по странам в JSON
        /// </summary>
        private async Task ExportCountryToJsonAsync(List<CountryIpDetails> countryStats)
        {
            var jsonPath = Path.Combine(_outputDirectory, "countries_statistics.json");
            var json = JsonConvert.SerializeObject(countryStats, Formatting.Indented);

            await File.WriteAllTextAsync(jsonPath, json);
            Console.WriteLine($"✓ Статистика по странам экспортирована в JSON: {jsonPath}");
        }

        /// <summary>
        /// Экспортировать статистику по городам в JSON
        /// </summary>
        private async Task ExportCityToJsonAsync(List<CityIpDetails> cityStats)
        {
            var jsonPath = Path.Combine(_outputDirectory, "cities_statistics.json");
            var json = JsonConvert.SerializeObject(cityStats, Formatting.Indented);

            await File.WriteAllTextAsync(jsonPath, json);
            Console.WriteLine($"✓ Статистика по городам экспортирована в JSON: {jsonPath}");
        }

        /// <summary>
        /// Экранировать значение для CSV
        /// </summary>
        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Если значение содержит запятую или кавычки, оборачиваем его в кавычки
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
