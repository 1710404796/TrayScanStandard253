namespace TrayScanStandard.Service.Models
{
    public class UploadDeviceErrorResponse
    {
        public string? code { get; set; } = string.Empty;
        public string? message { get; set; } = string.Empty;
        public bool? success { get; set; } = null;
        public string? category { get; set; } = string.Empty;
    }
}

