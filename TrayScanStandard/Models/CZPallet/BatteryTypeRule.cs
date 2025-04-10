/**************************************************************************
 * @file        BatteryTypeRule.cs
 * @author      scixing
 * @version     V1.0.0
 * @date        2022-12-18
 * @brief       lxWcsApp的标准HTTP通信服务器
 * @details
 * @copyright   Copyright (c) 2018-2024 杭州灵西机器人智能科技有限公司
 ***************************************************************************
 * @attention
 *
 ************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrayScanStandard.Models.CZPallet
{
    /// <summary>
    /// 电池类型规则
    /// </summary>
    [Obsolete("暂不使用")]
    public record BatteryTypeRule
    {

        public string Name { get; set; } = string.Empty;
        public string BatteryRegex { get; set; } = string.Empty;
        public BatteryTypeRule(string name, string batteryRegex)
        {
            Name = name;
            BatteryRegex = batteryRegex;
        }
        /// <summary>
        /// 检测电池条码规则 可能只需前13位
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckOk(string code)
        {
            return Regex.IsMatch(code, BatteryRegex);
        }

    }
}
