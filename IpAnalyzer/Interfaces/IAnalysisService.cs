namespace IpAnalyzer.Interfaces
{
    /// <summary>
    /// Интерфейс для анализа данных об IP адресах
    /// </summary>
    public interface IAnalysisService
    {
        /// <summary>
        /// Получить статистику по странам
        /// </summary>
        Task<IEnumerable<CountryStat>> GetCountryStatisticsAsync(IEnumerable<IpData> ipDataList);

        /// <summary>
        /// Получить города страны с наибольшим количеством IP адресов
        /// </summary>
        Task<IEnumerable<CityInfo>> GetCitiesForCountryWithMostIpsAsync(IEnumerable<IpData> ipDataList);
    }
}
