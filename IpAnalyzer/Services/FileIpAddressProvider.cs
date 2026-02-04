using System.Net;
using IpAnalyzer.Interfaces;

namespace IpAnalyzer.Services
{
    /// <summary>
    /// Реализация провайдера IP адресов из файла
    /// </summary>
    public class FileIpAddressProvider : IIpAddressProvider
    {
        private readonly string _filePath;

        public FileIpAddressProvider(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));

            _filePath = filePath;
        }

        /// <summary>
        /// Загрузить IP адреса из файла
        /// </summary>
        public async Task<IEnumerable<IPAddress>> GetAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Файл не найден: {_filePath}");

            try
            {
                var lines = await File.ReadAllLinesAsync(_filePath, ct);

                var ipAddresses = new List<IPAddress>();

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    // Пропускаем пустые строки и комментарии
                    if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith("#"))
                        continue;

                    // Парсим IP адрес
                    if (IPAddress.TryParse(trimmedLine, out var ipAddress))
                    {
                        ipAddresses.Add(ipAddress);
                    }
                    else
                    {
                        Console.WriteLine($"⚠️  Некорректный IP адрес: {trimmedLine}");
                    }
                }

                if (!ipAddresses.Any())
                    throw new InvalidOperationException("В файле не найдено IP адресов");

                return ipAddresses;
            }
            catch (Exception ex) when (!(ex is InvalidOperationException || ex is FileNotFoundException))
            {
                throw new InvalidOperationException($"Ошибка при чтении файла: {ex.Message}", ex);
            }
        }
    }
}
