namespace IpAnalyzer.Models
{
    /// <summary>
    /// Базовый класс для данных с подсчетом
    /// </summary>
    public abstract class CountedData
    {
        /// <summary>
        /// Количество IP адресов
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Процентное соотношение от общего количества
        /// </summary>
        public double Percentage { get; set; }
    }
}
