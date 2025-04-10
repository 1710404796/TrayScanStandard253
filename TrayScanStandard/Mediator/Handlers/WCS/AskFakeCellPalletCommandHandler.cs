using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinxUniverse.DI;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Service;

namespace TrayScanStandard.Mediator.Handlers.WCS
{
    internal class AskFakeCellPalletCommandHandler(XYLZPService xylZPService, CacheService cacheService, ILogger<AskFakeCellPalletCommandHandler> logger) : IRequestHandler<AskFakeCellPalletCommand>
    {
        public async Task Handle(AskFakeCellPalletCommand request, CancellationToken cancellationToken)
        {
            var pallet = Utils.CZPUtils.GetPalletByOutsideNum(request.StationNum);

            if (pallet == null)
            {
                //logger.LogWarning($"外部站台{request.StationNum}没有托盘");
                return;
            }
            Thread myThread = new Thread(async () =>
            {
                await xylZPService.WcsService.WaitForFakeCell(pallet.OutsideStationNum.ToString(), cancellationToken);
            });
            myThread.Start();
            //_ = Task.Factory.StartNew(async () => await xylZPService.WCSService.WaitForFakeCell(pallet.OutsideStationNum.ToString(), cacheService.Token), TaskCreationOptions.LongRunning);
            //;

            //var res = await xylZPService.WCSService.AskFakeCellResponse(new Service.Models.AskFakeCellRequest
            //{
            //    StationCode = pallet.OutsideStationNum.ToString(),
            //});
            //return res?.ContainerCode;
        }
    }
}
