namespace TrayScanStandard.Data.Models
{

    /// <summary>
    /// 报警日志
    /// </summary>
    public class WarningLog
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 报警时间
        /// </summary>
        public DateTime WarningTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 报警信息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 报警类型
        /// </summary>

        public WarningType WarningType { get; set; } = WarningType.None;

    }


    /// <summary>
    /// 报警类型
    /// </summary>
    public enum WarningType
    {
        None,
        Mes,
    }
}