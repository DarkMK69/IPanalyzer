using System.Net;
using System.Collections.Immutable;
using IpAnalyzer.Interfaces;
using IpAnalyzer.Models;

namespace IpAnalyzer.Services
{
    /// <summary>
    /// Сервис для обработки и анализа IP статистики
    /// </summary>
    public class IpStatisticsService : IIpStatisticsService
    {
        private readonly IEnumerable<IPAddress> _ipAddresses;
        private readonly IIpInfoClient _ipInfoClient;
        private readonly List<IpInfoDto> _ipInfoList;
        private ImmutableList<CountryIpDetails> _sortedCountryDetails = ImmutableList<CountryIpDetails>.Empty;
        private ImmutableList<CityIpDetails> _sortedCityDetails = ImmutableList<CityIpDetails>.Empty;

        public IpStatisticsService(IEnumerable<IPAddress> ipAddresses, IIpInfoClient ipInfoClient)
        {
            _ipAddresses = ipAddresses ?? throw new ArgumentNullException(nameof(ipAddresses));
            _ipInfoClient = ipInfoClient ?? throw new ArgumentNullException(nameof(ipInfoClient));
            _ipInfoList = new List<IpInfoDto>();
        }

        public ImmutableList<CountryIpDetails> SortedCountryIpDetails => _sortedCountryDetails;
        public ImmutableList<CityIpDetails> SortedCityIpDetails => _sortedCityDetails;

        /// <summary>
        /// Обработать все IP адреса и рассчитать статистику
        /// </summary>
        public async Task ProcessAsync()
        {
            await FetchIpInformationAsync();
            CalculateStatistics();
        }

        /// <summary>
        /// Получить информацию по всем IP адресам
        /// </summary>
        private async Task FetchIpInformationAsync()
        {
            Console.WriteLine("🌐 Получение информации об IP адресах...");

            foreach (var ip in _ipAddresses)
            {
                var ipInfo = await _ipInfoClient.GetInfoAsync(ip);
                if (ipInfo != null)
                {
                    _ipInfoList.Add(ipInfo);
                    Console.WriteLine($"  ✓ {ip} -> {ipInfo.Country}, {ipInfo.City}");
                }
                else
                {
                    Console.WriteLine($"  ✗ {ip} -> ошибка получения данных");
                }
            }

            Console.WriteLine($"✓ Обработано {_ipInfoList.Count} из {_ipAddresses.Count()} IP адресов\n");
        }

        /// <summary>
        /// Рассчитать статистику по странам и городам
        /// </summary>
        private void CalculateStatistics()
        {
            var countryStats = CalculateCountryStatistics();
            var cityStats = CalculateCityStatistics(countryStats);

            _sortedCountryDetails = ImmutableList.CreateRange(countryStats);
            _sortedCityDetails = ImmutableList.CreateRange(cityStats);
        }

        /// <summary>
        /// Рассчитать статистику по странам
        /// </summary>
        private List<CountryIpDetails> CalculateCountryStatistics()
        {
            var countryGroups = _ipInfoList
                .Where(ip => !string.IsNullOrWhiteSpace(ip.Country))
                .GroupBy(ip => ip.Country)
                .ToList();

            var totalCount = _ipInfoList.Count;

            return countryGroups
                .Select(group => new CountryIpDetails
                {
                    CountryCode = group.Key,
                    CountryName = group.Key, // Можно расширить для получения полного названия
                    Count = group.Count(),
                    Percentage = (group.Count() / (double)totalCount) * 100
                })
                .OrderByDescending(stat => stat.Count)
                .ToList();
        }

        /// <summary>
        /// Рассчитать статистику по городам страны с наибольшим количеством IP
        /// </summary>
        private List<CityIpDetails> CalculateCityStatistics(List<CountryIpDetails> countryStats)
        {
            var topCountry = countryStats.FirstOrDefault();
            if (topCountry == null)
                return new List<CityIpDetails>();

            var ipListForCountry = _ipInfoList
                .Where(ip => ip.Country == topCountry.CountryCode)
                .ToList();

            var totalCountryIps = ipListForCountry.Count;

            return ipListForCountry
                .Where(ip => !string.IsNullOrWhiteSpace(ip.City))
                .GroupBy(ip => new { City = ip.City, Region = ip.Region })
                .Select(group => new CityIpDetails
                {
                    City = group.Key.City,
                    Region = group.Key.Region,
                    CountryCode = topCountry.CountryCode,
                    Count = group.Count(),
                    Percentage = (group.Count() / (double)totalCountryIps) * 100
                })
                .OrderByDescending(city => city.Count)
                .ToList();
        }
    }
}
