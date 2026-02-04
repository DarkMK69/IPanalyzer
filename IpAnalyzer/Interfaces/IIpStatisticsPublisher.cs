namespace IpAnalyzer.Interfaces
{
    /// <summary>
    /// Интерфейс для публикации статистики IP адресов
    /// </summary>
    public interface IIpStatisticsPublisher
    {
        /// <summary>
        /// Опубликовать данные
        /// </summary>
        Task PublishAsync<T>(IEnumerable<T> data);
    }
}
