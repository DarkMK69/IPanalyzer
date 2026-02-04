using IpAnalyzer.Interfaces;
using IpAnalyzer.Models;

namespace IpAnalyzer.Services
{
    /// <summary>
    /// Реализация издателя статистики в Markdown файл
    /// </summary>
    public class MarkdownIpStatisticsPublisher : IIpStatisticsPublisher
    {
        private readonly string _outputDirectory;
        private readonly List<string> _reportContent;

        public MarkdownIpStatisticsPublisher(string outputDirectory = "output")
        {
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));
            _reportContent = new List<string>();

            // Создаем директорию, если её нет
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        /// <summary>
        /// Опубликовать данные в Markdown файл
        /// </summary>
        public async Task PublishAsync<T>(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
            {
                Console.WriteLine("⚠️  Нет данных для выгрузки в Markdown");
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
        /// Экспортировать статистику по странам в Markdown
        /// </summary>
        private async Task ExportCountryStatisticsAsync(IEnumerable<CountryIpDetails> countryStats)
        {
            var countryList = countryStats.ToList();
            var markdownPath = Path.Combine(_outputDirectory, "report.md");

            _reportContent.Clear();
            _reportContent.Add("# IP Адреса - Отчет анализа\n");
            _reportContent.Add($"**Дата генерации:** {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n");
            _reportContent.Add("## 📊 Статистика по странам\n");
            _reportContent.Add("| Код | Название | Количество IP | Процент |");
            _reportContent.Add("|-----|----------|---------------|---------|");

            foreach (var stat in countryList)
            {
                _reportContent.Add($"| {stat.CountryCode} | {stat.CountryName} | {stat.Count} | {stat.Percentage:F2}% |");
            }

            _reportContent.Add("");

            await File.WriteAllLinesAsync(markdownPath, _reportContent);
            Console.WriteLine($"✓ Отчет по странам добавлен в Markdown: {markdownPath}");
        }

        /// <summary>
        /// Экспортировать статистику по городам в Markdown
        /// </summary>
        private async Task ExportCityStatisticsAsync(IEnumerable<CityIpDetails> cityStats)
        {
            var cityList = cityStats.ToList();
            var markdownPath = Path.Combine(_outputDirectory, "report.md");

            // Добавляем к существующему файлу
            var existingContent = new List<string>();
            if (File.Exists(markdownPath))
            {
                existingContent.AddRange(await File.ReadAllLinesAsync(markdownPath));
            }

            var groupedByCountry = cityList.GroupBy(c => c.CountryCode);

            foreach (var countryGroup in groupedByCountry)
            {
                existingContent.Add($"## 🏙️  Города страны '{countryGroup.Key}'\n");
                existingContent.Add("| Город | Область | Количество IP | Процент |");
                existingContent.Add("|-------|---------|---------------|---------|");

                foreach (var city in countryGroup)
                {
                    var region = string.IsNullOrWhiteSpace(city.Region) ? "-" : city.Region;
                    existingContent.Add($"| {city.City} | {region} | {city.Count} | {city.Percentage:F2}% |");
                }

                existingContent.Add("");
            }

            await File.WriteAllLinesAsync(markdownPath, existingContent);
            Console.WriteLine($"✓ Отчет по городам добавлен в Markdown: {markdownPath}");
        }
    }
}
