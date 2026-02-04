namespace IpAnalyzer.Models
{
    /// <summary>
    /// Модель статистики по стране
    /// </summary>
    public class CountryIpDetails : CountedData
    {
        /// <summary>
        /// Код страны
        /// </summary>
        public string CountryCode { get; set; } = "";

        /// <summary>
        /// Название страны
        /// </summary>
        public string CountryName { get; set; } = "";
    }
}
