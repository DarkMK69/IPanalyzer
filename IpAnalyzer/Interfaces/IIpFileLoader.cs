namespace IpAnalyzer.Interfaces
{
    /// <summary>
    /// Интерфейс для загрузки IP адресов из файла
    /// </summary>
    public interface IIpFileLoader
    {
        /// <summary>
        /// Загрузить IP адреса из файла
        /// </summary>
        Task<IEnumerable<string>> LoadIpAddressesAsync(string filePath);
    }
}
