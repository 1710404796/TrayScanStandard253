using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using TrayScanStandard.Models;
using TrayScanStandard.Service;
using TrayScanStandard.Service.Models;

namespace TrayScanStandard.Mediator.Handlers.WCS
{
    /// <summary>
    /// 申请电芯信息命令处理程序
    /// </summary>
    /// <param name="xylZPService"></param>
    /// <param name="logger"></param>
    /// <param name="context"></param>
    internal class ApplyBatteryInfoCommandHandler : IRequestHandler<ApplyBatteryInfoCommand, BatteryLevel[]?>
    {
        private readonly XYLZPService _xylZPService;
        private readonly ILogger<ApplyBatteryInfoCommandHandler> _logger;
        private readonly LinxContext _context;
        private readonly Func<QueryAssembleInfoRequest, Task<Result<QueryAssembleInfoResponse>>> _applyMaterialFun;

        public ApplyBatteryInfoCommandHandler(XYLZPService xylZPService, ILogger<ApplyBatteryInfoCommandHandler> logger, LinxContext context)
        {
            _xylZPService = xylZPService;
            _logger = logger;
            _context = context;

            _applyMaterialFun = WcsService.MakeApi<QueryAssembleInfoRequest, Result<QueryAssembleInfoResponse>>(
                _xylZPService.WcsService.QueryAssembleInfoAsync,
                (data) => QueryAssembleInfoResponse.DebugDefault,
                () => MainStorage.Saves.ApiEnableTable.ApplyMaterialEnable
                );

            // 其他就以command形式测试？
        }

        public async Task<BatteryLevel[]?> Handle(ApplyBatteryInfoCommand request, CancellationToken cancellationToken)
        {

            var pallet = Utils.CZPUtils.GetPalletByOutsideNum(request.StationNum);

            if (pallet == null)
            {
                throw new Exception("托盘不存在");
            }
            // 要加访问模拟器模式
            var res = await _applyMaterialFun(new QueryAssembleInfoRequest
            {
                ContainerCode = pallet.PalletCode,
            });
            res.Match(
                Succ: (data) =>
                {
                    return data.Materials;

                },
                Fail: (f) =>
                {
                    _xylZPService.Mediator.Send(new WcsDataWarningBoxCommand(f.Message)).Wait();
                    throw f;
                }
            ).Iter(
                async m =>
                {
                    await pallet.BindBattery(m.Channel, m.MaterialCode, true, m.Level.GetBatteryLevelFromWcs());
                }
                );

            try
            {
                // 存一下托盘日志
                var palletlog = new PalletLog
                {
                    PalletCode = pallet.PalletCode,
                    BatteryInfo = pallet.Channels.Select(s => new BatteryInfo
                    {
                        BatteryCode = s.Code,
                        BatteryLevel = s.BatteryLevel,
                    }).Take(pallet.ChannelNum).ToList(),
                    Column = pallet.Column,
                    PalletType = pallet.Type

                };

                _context.Add(palletlog);

                await _context.SaveChangesAsync(cancellationToken);

            }
            catch (Exception ex)
            {

                //logger.LogError(ex, "组盘日志发生错误");
                _logger.LogError(ex, Properties.Resources.AnErrorOccurredInTheGroupDiskLog);
            }


            return [.. pallet.Channels.Take(pallet.ChannelNum).Select(channel => channel.BatteryLevel)];
        }
    }
}
