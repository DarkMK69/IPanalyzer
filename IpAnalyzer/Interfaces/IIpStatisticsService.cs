using System.Collections.Immutable;
using IpAnalyzer.Models;

namespace IpAnalyzer.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса обработки и анализа статистики IP адресов
    /// </summary>
    public interface IIpStatisticsService
    {
        /// <summary>
        /// Обработать данные и рассчитать статистику
        /// </summary>
        Task ProcessAsync();

        /// <summary>
        /// Отсортированные данные по странам
        /// </summary>
        ImmutableList<CountryIpDetails> SortedCountryIpDetails { get; }

        /// <summary>
        /// Отсортированные данные по городам
        /// </summary>
        ImmutableList<CityIpDetails> SortedCityIpDetails { get; }
    }
}
