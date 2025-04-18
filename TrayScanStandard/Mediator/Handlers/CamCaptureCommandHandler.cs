using Camera.Fs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Utils;

namespace TrayScanStandard.Mediator.Handlers
{
    public class CamCaptureCommandHandler : IRequestHandler<CamCaptureCommand, ImageData[]>
    {
        public Task<ImageData[]> Handle(CamCaptureCommand request, CancellationToken cancellationToken)
        {
            request.CaptureInfos.Map(
                s =>
                {
                  return DetectUtil.CaptureOne(s.Camera);
                }
                );
        }
    }

}
