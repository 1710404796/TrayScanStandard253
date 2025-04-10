
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TrayScanStandard.Mediator.Commands.CZP;
using TrayScanStandard.Service;


namespace TrayScanStandard.Mediator.Handlers.CZP
{
    public class ScanPalletCodeCommandHandler(XYLZPService xYLZPService) : IRequestHandler<ScanPalletCodeCommand, string>
    {
        public async Task<string> Handle(ScanPalletCodeCommand request, CancellationToken cancellationToken)
        {
            // Todo: 实现读取指定托盘码

            if (MainStorage.Saves.TestMode)
            {
                return $"{request.PalletIdx} TestCode";
            }
            else
            {
                var pallet = Utils.CZPUtils.GetPalletByIndex(request.PalletIdx);
                //if (pallet.OutsideStationNum == 2411)
                //{
                //    return "EU21L002AA40004";
                //}
                //else
                //{
                //    return "EU21L002AA40005";
                //}
                if (pallet == null)
                {
                    throw new Exception($"托盘{request.PalletIdx}");
                }


                throw new Exception($"未获取到托盘{request.PalletIdx}条码");

            }
        }
    }
}
