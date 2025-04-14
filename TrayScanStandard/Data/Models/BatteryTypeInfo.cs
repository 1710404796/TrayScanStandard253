using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.Data.Models
{
    
    public class BatteryTypeInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 配方名称
        /// </summary>
        public string TypeName { get; set; } = string.Empty;
        /// <summary>
        /// 通道数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        ///  列数
        /// </summary>
        public int Column { get; set; }
    }
}
