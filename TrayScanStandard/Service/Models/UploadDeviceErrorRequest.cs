using System;
using System.Text.Json.Serialization;
using static TrayScanStandard.MainStorage;

namespace TrayScanStandard.Service.Models
{
    public class UploadDeviceErrorRequest
    {
        [JsonPropertyName("systemCode")]
        public string SystemCode { get; set; } = Saves.StageSetting.SystemCode;

        [JsonPropertyName("houseCode")]
        public string HouseCode { get; set; } = Saves.StageSetting.HouseCode;

        [JsonPropertyName("equipNum")]
        public string EquipNum { get; set; } = Saves.StageSetting.LocationCode;

        [JsonPropertyName("recordDate")]
        public string RecordDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [JsonPropertyName("seatId")]
        public string SeatId { get; set; } = Saves.StageSetting.SeatId;

        [JsonPropertyName("guid")]
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();

        [JsonPropertyName("alarmEndTime")]
        public string AlarmEndTime { get; set; } = string.Empty;

        [JsonPropertyName("alarmType")]
        public string AlarmType { get; set; } = "停机";

        //[JsonPropertyName("alarmName")]
        //public string AlarmName => CodeDict.ContainsKey(FaultCode) ? CodeDict[FaultCode] : "未定义";

        [JsonPropertyName("alarmStartTime")]
        public string AlarmStartTime { get; set; } = string.Empty;

        [JsonPropertyName("faultCode")]
        public string FaultCode { get; set; } = string.Empty;
    }
}

