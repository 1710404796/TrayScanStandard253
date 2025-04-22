using Camera.Fs.Common;
using HKCamera.Fs.NET.Controls;
using MediatR;
using MugenCamera;
using System;
using System.Collections.Generic;
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
            var a = DetectUtil.UseLight( 
                () => request.CaptureInfos
                    .Map(s => s.ToEither("相机未初始化"))
                    .Traverse(s => s)
                    .Bind(c => 
                        c.AsParallel()
                        .Select(s =>
                        {
                            var aa = s.Exps.Map(e =>
                            {
                                s.Camera.SetControl(new AcquisitionControl { ExposureTime = (uint?)e });
                                return s.Camera.CaptureOne();

                            })
                            // 这段为无视拍照的错误
                            //.Choose(s => s.ToOption()).Apply(Either<string, IEnumerable<ImageData>>.Right);
                            .Traverse(s => s)
                            .Map(s => s.ToArray());
                            return aa;
                        })
                    .Traverse(s => s)
                    )
                );
            return a.Apply(Task.FromResult);

        }
    }

}
