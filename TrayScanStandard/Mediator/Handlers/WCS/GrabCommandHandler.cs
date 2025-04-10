using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Service;
using TrayScanStandard.Service.Models;



namespace TrayScanStandard.Mediator.Handlers.WCS
{
    internal class GrabCommandHandler : IRequestHandler<GrabCommand, bool>
    {
        private readonly XYLZPService _xylZPService;
        private readonly ILogger<GrabCommandHandler> _logger;
        private readonly Func<GrabRequest, Task<Result<GrabResponse>>> _fun;

        public GrabCommandHandler(XYLZPService xylZPService, ILogger<GrabCommandHandler> logger)
        {
            _xylZPService = xylZPService;
            _logger = logger;

            _fun = WcsService.MakeApi<GrabRequest, Result<GrabResponse>>(
                _xylZPService.WcsService.GrabAsync,
                (data) => GrabResponse.DebugDefault,
                () => MainStorage.Saves.ApiEnableTable.GrabDataUploadEnable
                );
        }

        public async Task<bool> Handle(GrabCommand request, CancellationToken cancellationToken)
        {
            if (MainStorage.Saves.TestMode)
            {
                return true;
            }
            var grabRequest = new GrabRequest
            {
                Materials = [.. request.Data.Infos.Select(m => new GrabMaterialInfo
                {
                    ChannelFrom = m.FromChannel,
                    ChannelTo = m.ToChannel,
                    MaterialCode = m.Code,
                })],
                ContainerFrom = request.Data.From.PalletCode,
                ContainerTo = request.Data.To.PalletCode,
                StationFrom = request.Data.From.OutsideStationNum.ToString(),
                StationTo = request.Data.To.OutsideStationNum.ToString(),


            };
            var data = await _xylZPService.WcsService.GrabAsync(grabRequest);

            if (data == null) { return false; }
            return true;
        }
    }
}
