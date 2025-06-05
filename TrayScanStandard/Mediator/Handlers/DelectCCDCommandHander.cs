using Camera.Fs.Common;
using LanguageExt.Common;
using LinxUniverse.Algo.Common;
using LinxUniverse.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using MugenCodeDetecter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Mediator.Queries;
using TrayScanStandard.Service;
using TrayScanStandard.ViewModel;
using VMWebAIClient;

namespace TrayScanStandard.Mediator.Handlers
{
    internal class DelectCCDCommandHander(ScanCameraService scanCameraService, ILogger<ScanCameraService> logger, IMediator mediator
        , IVMWebAIClient vMWebAIClient,
        ImageDisplayViewModel imageDisplayViewModel
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
            //data.IfRight(s => { Console.WriteLine(s.Count()); });

            var dataFilenames = data.Map((IEnumerable<ImageData[]> d) =>
                                        d.Map(
                                            (ci, s) =>
                                                s.Map((ei, s1) =>
                                                {
                                                    var name = $"{FilenameHelper.AppPath}Data2D\\{FilenameHelper.FileName}{ci}_{ei}.png";
                                                    File.WriteAllBytes(name, s1.Data);
                                                    return name;
                                                })
                                            )
                                    );
                ;
           
            //dataFilenames.IfRight(s => { Console.WriteLine(s.Count()); });
            var res = (await dataFilenames.BindAsync(
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
                                .Map(s =>
                                {
                                    return vMWebAIClient.DetectCodesAsync(s.s, s.Item2);
                                })
                                .TraverseSerial(s => s)
                                .Map(s =>
                                    s.Traverse(s1 => s1.Codes)
                                    .Map(s1 => s1.SelectMany(s => s))
                                )
                            //.Map(s => s.SelectMany(d => d.Codes).DistinctBy(d => d.Index)) // 看看要不要考虑重复位置
                            )
                        .TraverseSerial(s => s)
                        .Map(s =>
                                s.Traverse(s1 => s1)
                                    //.Map(s1 => s1.SelectMany(s2 => s2))
                        )
                    // 这里merge一下
                    ))
                    //.Map(res => res.DistinctBy(s => s.Index))

                ;
            res.Iter(s =>
                    s.Zip(scanCameraService.Image2DViewModels)
                    .Iter(d =>
                    {
                        d.Item2.TempResult = new CodeDetectResult(d.Item1.ToArr());
                    })
            );
            
            var res1 = res.Map(r => 
                r.SelectMany(s => s)
                .DistinctBy(s => s.Index)
                ).Map(s => new DetectResult(s.ToArr()));
            // 清空一下？
            imageDisplayViewModel.XYLStation.ClearStage();
            res1.Iter(s =>
                s.Channels.Iter
                    ( async c => await imageDisplayViewModel.XYLStation.BindBattery(c.Index - 1, c.Code, true, Models.BatteryLevel.OK)
                    )
             );

            dataFilenames.IfRight(f =>
            {
                var d = f.Select(s => s.First());

                mediator.Send(new PushImgCommand(d.ToArray()));

            });
            return res1;
            //).Traverse(s => s)
            //    ));
        }
    }
}
