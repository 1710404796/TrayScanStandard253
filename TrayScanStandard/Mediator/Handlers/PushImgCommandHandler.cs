using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                var viewModel = scanCameraService.Image2DViewModels[i];
                var imagePath = request.Imgs[i];
                viewModel.tempImg = File.ReadAllBytes(imagePath);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.SetResultImage(imagePath);
                    viewModel.Update();
                    viewModel.UpdateResult();
                });
            }
            return Task.CompletedTask;
        }
    }
}
