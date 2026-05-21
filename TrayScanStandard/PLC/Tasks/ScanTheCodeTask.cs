using LinxUniverse.PLC.Common.Models;
using LinxUniverse.PLCProtos;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Service;

namespace TrayScanStandard.PLC.Tasks
{
    /// <summary>
    /// 扫码任务（PLC action=1）
    /// 到位后触发扫码 -> 上报WCS BatteryCheck -> WCS返回成功后反馈PLC
    /// </summary>
    [S7TaskDb(500, 501)]
    public class ScanTheCodeTask : CoreTask<CCDContext>
    {
        private static readonly HttpClient HttpClient = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ScanTheCodeTask() : base(1)
        {
            TaskName = "扫码任务";
        }

        public override async Task<bool> DoSth()
        {
            if (MainStorage.SelectBattery == null)
            {
                Logger?.LogError("扫码任务失败: 未选择电池配方");
                return false;
            }

            var detectResult = await Context.Mediator.Send(new DetectCCDCommand(MainStorage.SelectBattery), CTS.Token);
            if (detectResult.IsLeft)
            {
                detectResult.IfLeft(err => Logger?.LogError($"扫码任务失败: {err}"));
                return false;
            }

            var channelCodeMap = detectResult.Match(
                Right: r => r.Channels.ToDictionary(c => c.Index, c => c.Code ?? string.Empty),
                Left: _ => new Dictionary<int, string>());

            var request = new BatteryCheckRequest
            {
                SystemCode = MainStorage.Saves.SystemCode,
                HouseCode = MainStorage.Saves.HouseCode,
                DeviceCode = MainStorage.Saves.DeviceCode,
                ContainerCode = string.Empty,
                Parameters = new { },
                BatteryList = Enumerable.Range(1, MainStorage.SelectBattery.Count)
                    .Select(i => new BatteryCheckRequest.BatteryItem
                    {
                        Channel = i.ToString(),
                        BatteryCode = channelCodeMap.GetValueOrDefault(i, string.Empty)
                    })
                    .ToList()
            };

            var url = BuildBatteryCheckUrl();
            if (string.IsNullOrWhiteSpace(url))
            {
                Logger?.LogError("扫码任务失败: 无法根据WCS IP/端口生成BatteryCheck地址");
                return false;
            }

            try
            {
                var requestJson = JsonSerializer.Serialize(request, JsonOptions);
                Logger?.LogInformation($"调用WCS BatteryCheck, Url={url}, Request={requestJson}");

                using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                using var response = await HttpClient.PostAsync(url, content, CTS.Token);
                var responseText = await response.Content.ReadAsStringAsync(CTS.Token);

                if (!response.IsSuccessStatusCode)
                {
                    Logger?.LogError($"WCS BatteryCheck调用失败: Status={response.StatusCode}, Response={responseText}");
                    return false;
                }

                using var responseDoc = JsonDocument.Parse(responseText);
                var root = responseDoc.RootElement;
                var responseCode = -1;
                var hasCode = root.TryGetProperty("responseCode", out var codeElement)
                    && codeElement.TryGetInt32(out responseCode);
                var responseMessage = root.TryGetProperty("responseMessage", out var messageElement)
                    ? messageElement.GetString()
                    : string.Empty;

                if (!hasCode)
                {
                    Logger?.LogError($"WCS BatteryCheck响应解析失败: {responseText}");
                    return false;
                }

                if (responseCode != 0)
                {
                    Logger?.LogError($"WCS BatteryCheck业务失败: Code={responseCode}, Message={responseMessage}");
                    return false;
                }

                Logger?.LogInformation($"WCS BatteryCheck成功: {responseText}");
                return true;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "调用WCS BatteryCheck出现异常");
                return false;
            }
        }

        private static string BuildBatteryCheckUrl()
        {
            var ip = MainStorage.Saves.IP?.Trim() ?? string.Empty;
            var portRaw = MainStorage.Saves.Port?.Trim() ?? string.Empty;

            // 兼容用户把“ip:port”填到端口框里的旧习惯。
            if (string.IsNullOrWhiteSpace(ip) && portRaw.Contains(':'))
            {
                var parts = portRaw.Split(':', 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    ip = parts[0];
                    portRaw = parts[1];
                }
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                return string.Empty;
            }

            var scheme = ip.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || ip.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : "http://";

            var hostText = $"{scheme}{ip}";
            if (!Uri.TryCreate(hostText, UriKind.Absolute, out var baseUri))
            {
                return string.Empty;
            }

            var builder = new UriBuilder(baseUri);
            if (int.TryParse(portRaw, out var port))
            {
                builder.Port = port;
            }
            builder.Path = "restful/API/V3/Wcs2Wms/batteryCheck";
            return builder.Uri.ToString();
        }
    }
}
