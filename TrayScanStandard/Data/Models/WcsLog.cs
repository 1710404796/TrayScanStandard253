using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.Data.Models
{
    public class WcsLog
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// API名称
        /// </summary>
        public string ApiName { get; set; } = string.Empty;

        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime RequestTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 响应时间
        /// </summary>
        public DateTime ResponseTime { get; set; }
        /// <summary>
        /// 请求信息
        /// </summary>
        public string Request { get; set; } = string.Empty;
        /// <summary>
        /// 响应信息
        /// </summary>
        public string Response { get; set; } = string.Empty;

        /// <summary>
        /// 是否成功
        /// </summary>
        [DefaultValue(true)]
        public bool IsSuccess { get; set; } = true;
    }
}
