using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Service;


namespace TrayScanStandard.Mediator.Handlers.WCS
{
    public class DeviceErrorCommandHandler(IMediator mediator, XYLZPService xylZPService) : IRequestHandler<DeviceErrorCommand>
    {
        public async Task Handle(DeviceErrorCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = request.ErrorCode == 0 ? string.Empty : $"{Properties.Resources.Error},{request.ErrorCode}";
            await mediator.Send(new PushErrorMessageCommand(errorMessage));
            await xylZPService.WcsService.DeviceErrorAsync(new Service.Models.DeviceErrorRequest
            {
                ErrorCode = request.ErrorCode.ToString(),
                ErrorMessage = errorMessage
            });
        }
    }
}
