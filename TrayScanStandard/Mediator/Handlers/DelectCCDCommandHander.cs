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
using VMWebAIClient;
using Camera.Fs.Common;
using System.IO;
using LinxUniverse.Utils;

namespace TrayScanStandard.Mediator.Handlers
{
    internal class DelectCCDCommandHander(ScanCameraService scanCameraService, ILogger<ScanCameraService> logger, IMediator mediator
        , IVMWebAIClient vMWebAIClient
        )
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

            // 将图片存为文件

            var dataFilenames = data.Map((IEnumerable<ImageData[]> d) => 
                                        d.Map(
                                            (ci, s) => 
                                                s.Map((ei, s1) =>
                                                {
                                                    var name = $"{FilenameHelper.AppPath}Data2D\\{ci}_{ei}.png";
                                                    File.WriteAllBytes(name, s1.Data);
                                                    return name;
                                                })
                                            )
                                    );


            var res = dataFilenames.Bind(
                    camImgs => 
                        camImgs
                        .Zip(request.BatteryTypeInfo.Regions.Take(MainStorage.Saves.CameraCnt))
                        .Map(
                            imgs => imgs.Item1
                                           .Map(s =>
                                                    (
                                                            s,
                                                            imgs.Item2
                                                                .Map(s => s.ToROI())
                                                                .ToArray()
                                                        )
                                                   )
                                              .Map(s => vMWebAIClient.DetectCodesAsync(s.s, s.Item2).Result
                                              )
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
