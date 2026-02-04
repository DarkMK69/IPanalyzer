using Microsoft.Extensions.DependencyInjection;
using IpAnalyzer.Interfaces;
using IpAnalyzer.Services;

namespace IpAnalyzer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {

                var services = new ServiceCollection();
                ConfigureServices(services);

                var serviceProvider = services.BuildServiceProvider();

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent?.FullName ?? ".";
                string ipFilePath = Path.Combine(projectRoot, "ip_addresses.txt");
                

                if (!File.Exists(ipFilePath))
                {
                    ipFilePath = "ip_addresses.txt";
                }

                Console.WriteLine("Загрузка IP адресов из файла...");
                var addressProvider = new FileIpAddressProvider(ipFilePath);
                var ipAddresses = await addressProvider.GetAsync();
                Console.WriteLine($"✓ Загружено {ipAddresses.Count()} IP адресов\n");


                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                var ipInfoClient = new HttpInfoClient(httpClient, "https://ipinfo.io");

                var statisticsService = new IpStatisticsService(ipAddresses, ipInfoClient);

                await statisticsService.ProcessAsync();

                var publisher = new ConsoleIpStatisticsPublisher();

                Console.WriteLine();
                await publisher.PublishAsync(statisticsService.SortedCountryIpDetails);
                await publisher.PublishAsync(statisticsService.SortedCityIpDetails);


                var filePublisher = new FileIpStatisticsPublisher(Path.Combine(projectRoot, "output"));
                Console.WriteLine("\n Выгрузка данных в файлы...");
                await filePublisher.PublishAsync(statisticsService.SortedCountryIpDetails);
                await filePublisher.PublishAsync(statisticsService.SortedCityIpDetails);

                var markdownPublisher = new MarkdownIpStatisticsPublisher(Path.Combine(projectRoot, "output"));
                Console.WriteLine("\n Генерация Markdown отчета...");
                await markdownPublisher.PublishAsync(statisticsService.SortedCountryIpDetails);
                await markdownPublisher.PublishAsync(statisticsService.SortedCityIpDetails);

                Console.WriteLine("\n Анализ завершен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Настройка сервисов для DI контейнера
        /// </summary>
        private static void ConfigureServices(ServiceCollection services)
        {
            // HttpClientFactory для создания HttpClient
            services.AddHttpClient();
        }
    }
}
