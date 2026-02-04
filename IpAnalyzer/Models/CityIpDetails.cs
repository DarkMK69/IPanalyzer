namespace IpAnalyzer.Models
{
    /// <summary>
    /// Модель статистики по городу
    /// </summary>
    public class CityIpDetails : CountedData
    {
        /// <summary>
        /// Название города
        /// </summary>
        public string City { get; set; } = "";

        /// <summary>
        /// Область/регион
        /// </summary>
        public string Region { get; set; } = "";

        /// <summary>
        /// Код страны
        /// </summary>
        public string CountryCode { get; set; } = "";
    }
}
