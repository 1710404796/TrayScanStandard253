using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Service;

namespace TrayScanStandard.Mediator.Handlers
{
    internal class PushImgCommandHandler(ScanCameraService scanCameraService) : IRequestHandler<PushImgCommand>
    {
        public Task Handle(PushImgCommand request, CancellationToken cancellationToken)
        {
            for (int i = 0; i < request.Imgs.Length; i++)
            {
                scanCameraService.Image2DViewModels[i].ResultImg = request.Imgs[i];
                scanCameraService.Image2DViewModels[i].Update();
                scanCameraService.Image2DViewModels[i].UpdateResult();
            }
            return Task.CompletedTask;
        }
    }
}
