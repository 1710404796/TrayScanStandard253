using Camera.Fs.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.Mediator.Commands
{
    // 要有单相机拍照的最好
    public record CamCaptureCommand: IRequest<ImageData>;
}
