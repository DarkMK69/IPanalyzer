using System.Net;

namespace IpAnalyzer.Interfaces
{
    /// <summary>
    /// Интерфейс для получения IP адресов из источника данных
    /// </summary>
    public interface IIpAddressProvider
    {
        /// <summary>
        /// Получить список IP адресов асинхронно
        /// </summary>
        Task<IEnumerable<IPAddress>> GetAsync(CancellationToken ct = default);
    }
}
