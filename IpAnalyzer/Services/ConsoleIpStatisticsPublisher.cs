using IpAnalyzer.Interfaces;
using IpAnalyzer.Models;

namespace IpAnalyzer.Services
{
    /// <summary>
    /// Реализация издателя статистики в консоль
    /// </summary>
    public class ConsoleIpStatisticsPublisher : IIpStatisticsPublisher
    {
        /// <summary>
        /// Опубликовать данные в консоль
        /// </summary>
        public Task PublishAsync<T>(IEnumerable<T> data)
        {
            if (data == null || !data.Any())
            {
                Console.WriteLine("Нет данных для вывода");
                return Task.CompletedTask;
            }

            PrintData(data);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Вывести данные в консоль в зависимости от типа
        /// </summary>
        private void PrintData<T>(IEnumerable<T> data)
        {
            var firstItem = data.FirstOrDefault();

            if (firstItem is CountryIpDetails countryData)
            {
                PrintCountryStatistics((IEnumerable<CountryIpDetails>)(object)data);
            }
            else if (firstItem is CityIpDetails cityData)
            {
                PrintCityStatistics((IEnumerable<CityIpDetails>)(object)data);
            }
            else
            {
                // Fallback для других типов
                foreach (var item in data)
                {
                    Console.WriteLine(item);
                }
            }
        }

        /// <summary>
        /// Вывести статистику по странам
        /// </summary>
        private void PrintCountryStatistics(IEnumerable<CountryIpDetails> countryStats)
        {
            Console.WriteLine("Статистика по странам:");
            Console.WriteLine(new string('-', 70));
            Console.WriteLine("{0,-20} | {1,-15} | {2,10} | {3,8}", 
                "Код", "Количество", "Процент", "");

            foreach (var stat in countryStats)
            {
                Console.WriteLine("{0,-20} | {1,15} | {2,7:F2}% | {3,8}", 
                    stat.CountryCode, stat.Count, stat.Percentage, "");
            }
            Console.WriteLine(new string('-', 70) + "\n");
        }

        /// <summary>
        /// Вывести статистику по городам
        /// </summary>
        private void PrintCityStatistics(IEnumerable<CityIpDetails> cityStats)
        {
            var groupedByCountry = cityStats.GroupBy(c => c.CountryCode);

            foreach (var countryGroup in groupedByCountry)
            {
                Console.WriteLine($"🏙️  Города страны '{countryGroup.Key}':");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine("{0,-40} | {1,-20} | {2,10} | {3,8}", 
                    "Город", "Область", "Количество", "Процент");

                foreach (var city in countryGroup)
                {
                    var regionInfo = string.IsNullOrWhiteSpace(city.Region) ? "-" : city.Region;
                    Console.WriteLine("{0,-40} | {1,-20} | {2,10} | {3,7:F2}%", 
                        city.City, regionInfo, city.Count, city.Percentage);
                }
                Console.WriteLine(new string('-', 80) + "\n");
            }
        }
    }
}
