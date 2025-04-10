using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static TrayScanStandard.MainStorage;

namespace TrayScanStandard.Service.Models
{
    public class MaterialInStationRequests
    {
        /// <summary>
        /// 系统编码
        /// 用于判断由哪个系统发出的任务请求
        /// </summary>
        [Required]
        [JsonPropertyName("systemCode")]
        public string SystemCode { get; set; } = Saves.StageSetting.SystemCode;
        /// <summary>
        /// 仓库编码
        /// 用于判断由哪个库发出的任务请求
        /// </summary>
        [Required]
        [JsonPropertyName("houseCode")]
        public string HouseCode { get; set; } = Saves.StageSetting.HouseCode;
        /// <summary>
        /// 扩展项
        /// 字段不够时可用拓展性补充
        /// </summary>
        [Required]
        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; } = new();

        /// <summary>
        /// 设备编号
        /// 1001
        /// </summary>
        [Required]
        [JsonPropertyName("deviceCode")]
        public string DeviceCode { get; set; } = Saves.StageSetting.DeviceCode;

        [Required]
        [JsonPropertyName("skuCodes")]
        /// <summary>
        /// 物料信息
        /// </summary>
        public List<string> Materials { get; set; } = new();
        [Required]
        [JsonPropertyName("processCode")]
        public string ProcessCode { get; set; } = Saves.StageSetting.ProcessCode;
    }
    public class MaterialInStationResponse
    {
        /// <summary>
        /// 请求结果状态码
        /// 0-正常；非0-异常
        /// </summary>
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; } = 0;
        /// <summary>
        /// 请求结果描述
        /// </summary>
        [JsonPropertyName("statusMessage")]
        public string? StautsMessage { get; set; }
        /// <summary>
        /// 扩展项
        /// 字段不够时可用拓展性补充
        /// </summary>
        [Required]
        [JsonPropertyName("parameters")]
        public Dictionary<string, string>? Parameters { get; set; } = new();
        /// <summary>
        /// 物料信息
        /// </summary>
        [JsonPropertyName("inStationCellInfos")]
        public List<MaterialInStationReturnInfo> InStationCellInfo { get; set; } = new();
    }


    public class MaterialInStationReturnInfo
    {
        [JsonPropertyName("skuCode")]
        public string? SkuCode { get; set; }
        /// <summary>
        /// 物料状态
        /// 暂定ok为0 ； ng 为 1 其他为扩展
        /// </summary>
        [JsonPropertyName("status")]
        public string? StatusCode { get; set; } = "0";
        /// <summary>
        /// 物料类型
        /// </summary>
        [JsonPropertyName("skuType")]
        public string? MaterialType { get; set; }

        /// <summary>
        /// 物料状态
        /// </summary>
        [JsonPropertyName("level")]
        public string? Level { get; set; }
        [JsonPropertyName("productionData")]

        public Dictionary<string, string> ProductionData { get; set; } = new();

        [JsonPropertyName("remark")]
        public string? Remark { get; set; }
    }
}
