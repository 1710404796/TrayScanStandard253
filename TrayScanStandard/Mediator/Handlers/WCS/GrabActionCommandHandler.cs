using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Models.CZPallet;
using TrayScanStandard.Service;
using TrayScanStandard.Service.Models;


namespace TrayScanStandard.Mediator.Handlers.WCS
{
    public class GrabActionCommandHandler : IRequestHandler<GrabActionCommand, bool>
    {
        private readonly XYLZPService _xylZPService;
        private readonly LinxContext _context;
        /// <summary>
        /// 
        /// </summary>
        private readonly Func<GrabActionRequest, Task<Result<GrabActionResponse>>> _palletInsertFunc;
        private readonly Func<GrabActionRequest, Task<Result<GrabActionResponse>>> _palletRemoveFunc;

        public GrabActionCommandHandler(XYLZPService xylZPService, ILogger<GrabActionCommandHandler> logger, LinxContext context)
        {
            _xylZPService = xylZPService;
            _context = context;

            _palletInsertFunc = WcsService.MakeApi<GrabActionRequest, Result<GrabActionResponse>>(
                _xylZPService.WcsService.GrabActionAsync,
                (data) => GrabActionResponse.DebugDefault,
                () => MainStorage.Saves.ApiEnableTable.GrabActionInsertEnable
                );
            _palletRemoveFunc = WcsService.MakeApi<GrabActionRequest, Result<GrabActionResponse>>(
                _xylZPService.WcsService.GrabActionAsync,
                (data) => GrabActionResponse.DebugDefault,
                () => MainStorage.Saves.ApiEnableTable.GrabActionRemoveEnable
                );
        }

        public async Task<bool> Handle(GrabActionCommand request, CancellationToken cancellationToken)
        {
            var pallet = Utils.CZPUtils.GetPalletByOutsideNum(request.StationNum);

            if (pallet == null)
            {
                //xylZPService.Mediator.Send(new LogCommand($"外部站台{request.StationNum}没有托盘"));
                return false;
            }

            ActionType actionType = (request.Start, pallet.Type) switch
            {
                (true, PalletType.组盘) => ActionType.insertStart,
                (true, PalletType.拆盘 or PalletType.筛选) => ActionType.removeStart,
                (false, PalletType.组盘) => ActionType.insertFinish,
                (false, PalletType.拆盘 or PalletType.筛选) => ActionType.removeFinish,
                _ => ActionType.removeFinish
            };

            var response = await
                (actionType switch
                {
                    ActionType.insertStart or ActionType.insertFinish => _palletInsertFunc,
                    ActionType.removeStart or ActionType.removeFinish => _palletRemoveFunc,
                    _ => throw new NotImplementedException()
                })
            (new GrabActionRequest
            {
                ContainerCode = pallet.PalletCode,
                StationCode = pallet.OutsideStationNum.ToString(),
                SelfActionType = actionType,
            });

            var res = response.Match(
                Succ: s => true,
                Fail: f =>
                {
                    _xylZPService.Mediator.Send(new WcsDataWarningBoxCommand(f.Message)).Wait();
                    throw f;
                }
                );
            //SaveLog:

            if (!request.Start && pallet.Type == PalletType.组盘)
            {
                // 存一下托盘日志
                var palletlog = new PalletLog
                {
                    PalletCode = pallet.PalletCode,
                    BatteryInfo = [.. pallet.Channels.Select(s => new BatteryInfo
                    {
                        BatteryCode = s.Code,
                        BatteryLevel = s.BatteryLevel,
                    }).Take(pallet.ChannelNum)],
                    Column = pallet.Column,
                    PalletType = pallet.Type

                };

                _context.Add(palletlog);

                await _context.SaveChangesAsync(cancellationToken);

            }


            return res;
        }
    }

    //public enum ActionType
    //{
    //    insertStart,
    //    removeStart,
    //    insertFinish,
    //    removeFinish,
    //}

}
