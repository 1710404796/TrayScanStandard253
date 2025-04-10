using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Service;

namespace TrayScanStandard.Mediator.Handlers.WCS
{
    internal class MoveFakeCellPalletCommandHandler(XYLZPService xylZPService, ILogger<MoveFakeCellPalletCommandHandler> logger) : IRequestHandler<MoveFakeCellPalletCommand, bool>
    {
        public async Task<bool> Handle(MoveFakeCellPalletCommand request, CancellationToken cancellationToken)
        {
            var pallet = Utils.CZPUtils.GetPalletByOutsideNum(request.StationNum);

            if (pallet == null)
            {
                //logger.LogWarning($"外部站台{request.StationNum}没有托盘");
                return default;
            }
            var res = await xylZPService.WcsService.MoveFakeCellResponse(new Service.Models.MoveFakeCellRequest
            {
                StationCode = pallet.OutsideStationNum.ToString(),
                ContainerCode = pallet.PalletCode
            });
            return true;
        }
    }
}
