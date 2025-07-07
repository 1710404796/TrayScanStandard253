


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace TestProto.Controllers
{

    public enum ErrorType
    {
        Successed,
        CameraError,
        SomeResultError,
    }

    public class QRCodeResult
    {
        public ErrorType ErrorCode { get; set; }
        public Dictionary<int, string> Data { get; set; } = [];
    }


    [JsonSerializable(typeof(QRCodeResult))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
