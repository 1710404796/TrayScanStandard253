using System;
using System.Text.Json.Serialization;
using static TrayScanStandard.MainStorage;

namespace TrayScanStandard.Service.Models
{
    public class UploadDeviceStatusRequest
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
        public string Guid { get; set; } = string.Empty;

        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyName("uploadTime")]
        public string UploadTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    }
}