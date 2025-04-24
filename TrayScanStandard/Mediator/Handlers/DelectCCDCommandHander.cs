using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Service;
using MugenCodeDetecter;

namespace TrayScanStandard.Mediator.Handlers
{
    internal class DelectCCDCommandHander(ScanCameraService scanCameraService, ILogger<ScanCameraService> logger, IMediator mediator) : IRequestHandler<DelectCCDCommand, DelectResult>
    // 这个就是默认全部的
    {
        public async Task<DelectResult> Handle(DelectCCDCommand request, CancellationToken cancellationToken)
        {
            var captureInfos = scanCameraService.MugenCameras
                .Zip(scanCameraService.Image2DViewModels.Select(s => s.CameraSetting))
                .Map(s => s.Item1.Map(c => new CaptureInfo(c, s.Item2.Exposure))).ToArray();
            var data = await mediator.Send(new CamCaptureCommand(captureInfos));
            //data.Map(s => s.Select(p =>
            //    MainStorage.Algo.Bind(s => s.DetectCodes(p))


            //).Traverse(s => s)
            //    ));
        }
    }
}
