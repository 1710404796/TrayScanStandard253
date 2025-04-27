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
using LinxUniverse.Algo.Common;
using TrayScanStandard.Mediator.Queries;

namespace TrayScanStandard.Mediator.Handlers
{
    internal class DelectCCDCommandHander(ScanCameraService scanCameraService, ILogger<ScanCameraService> logger, IMediator mediator)
        : IRequestHandler<DelectCCDCommand, Either<string, DetectResult>>
    // 这个就是默认全部的
    {
        public async Task<Either<string, DetectResult>> Handle(DelectCCDCommand request, CancellationToken cancellationToken)
        {

            if (request.BatteryTypeInfo == null)
            {
                return Either<string, DetectResult>.Left("电池信息为空");
            }
            var captureInfos = scanCameraService.MugenCameras
                .Zip(scanCameraService.Image2DViewModels.Select(s => s.CameraSetting))
                .Map(s => s.Item1.Map(c => new CaptureInfo(c, s.Item2.Exposure))).ToArray();
            var data = await mediator.Send(new CamCaptureCommand(captureInfos));
            var res = data.Bind(
                    camImgs => 
                        camImgs
                        .Zip(request.BatteryTypeInfo.Regions.Take(MainStorage.Saves.CameraCnt))
                        .Map(
                            imgs => imgs.Item1.Map(s =>
                                                    new DetectParam(
                                                            s.Data, 
                                                            imgs.Item2
                                                                .Map(s => s.ToROI())
                                                                .ToArray()
                                                        )
                                                   )
                                              .Map(s => MainStorage.Algo.Bind(a => a.DetectCodes(s)))
                                              .Traverse(s => s)
                                              .Map(s => s.SelectMany(d => d.Codes).DistinctBy(d => d.Index)) // 看看要不要考虑重复位置
                            )
                        .Traverse(s=>s)
                        // 这里merge一下
                    );
            res.Iter(s =>
                    s.Zip(scanCameraService.Image2DViewModels)
                    .Iter(d =>
                    {
                        d.Item2.TempResult = new CodeDetectResult(d.Item1.ToArr());
                    })
            );

            return res.Map(r => 
                r.SelectMany(s => s)
                .DistinctBy(s => s.Index)
                ).Map(s => new DetectResult(s.ToArr()));
            //).Traverse(s => s)
            //    ));
        }
    }
}
