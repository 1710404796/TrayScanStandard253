using Camera.Fs.Common;
using HKCamera.Fs.NET.Controls;
using MediatR;
using MugenCamera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Utils;

namespace TrayScanStandard.Mediator.Handlers
{
    // 此处用于处理相机拍照的命令
    public class CamCaptureCommandHandler : IRequestHandler<CamCaptureCommand, Either<string, IEnumerable<ImageData[]>>>
    {
        public Task<Either<string, IEnumerable<ImageData[]>>> Handle(CamCaptureCommand request, CancellationToken cancellationToken)
        {
            //var a = request.CaptureInfos.Select(s => 1);
            if (!MainStorage.Saves.CameraEnable)
            {
                return Either<string, IEnumerable<ImageData[]>>.Right
               ([..request.CaptureInfos
               .Select(s => new ImageData[]
                       { new ImageData(File.ReadAllBytes(@"D:\testImg\right.png"))
                       }
                 )
               ]).Apply(Task.FromResult);
            }

            var a = DetectUtil.UseLight(
                () => request.CaptureInfos
                    .Map(s => s.ToEither("相机未初始化"))
                    .Traverse(s => s)
                    .BindAsync(c =>
                        c
                        // .AsParallel()
                        // .AsOrdered()
                        .Select(s => Task.Run(() => ProcessCaptureInfo(s)))
                        .Apply(Task.WhenAll)
                        .Map(s => s.Traverse(q => q))
                    //.TraverseParallel(s => s)
                    )
                );//.Apply(Task.FromResult);

            // 本地函数：处理单个CaptureInfo
            Either<string, ImageData[]> ProcessCaptureInfo(CaptureInfo s)
            {
                var aa = s.Exps.Map(e => 
                        s.Camera
                        .SetControl(new AcquisitionControl { ExposureTime = e })
                        .Bind(DetectUtil.CaptureOne)
                    )
                .Traverse(s => s)
                .Map(s => s.ToArray());
                return aa;
            }
        }
    }

}
