using System.Text.Json.Serialization;

namespace TrayScanStandard.Service
{
    public sealed class BatteryCheckRequest
    {
        [JsonPropertyName("systemCode")]
        public string SystemCode { get; set; } = MainStorage.Saves.SystemCode;

        [JsonPropertyName("houseCode")]
        public string HouseCode { get; set; } = MainStorage.Saves.HouseCode;

        [JsonPropertyName("deviceCode")]
        public string DeviceCode { get; set; } = MainStorage.Saves.DeviceCode;

        /// <summary>
        /// Õ–≈Ã∫≈
        /// </summary>
        [JsonPropertyName("containerCode")]
        public string ContainerCode { get; set; } = string.Empty;

        [JsonPropertyName("parameters")]
        public object Parameters { get; set; } = new { };

        [JsonPropertyName("batteryList")]
        public List<BatteryItem> BatteryList { get; set; } = [];

        public sealed class BatteryItem
        {
            [JsonPropertyName("batteryCode")]
            public string BatteryCode { get; set; } = string.Empty;

            [JsonPropertyName("channel")]
            public string Channel { get; set; } = string.Empty;
        }
    }

    public sealed class BatteryCheckResponse : IWcsResponse
    {
        public int ResponseCode { get; set; }

        public string? ResponseMessage { get; set; }

        public string? Parameters { get; set; }
    }
}
