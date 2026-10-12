using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Models;
using TrayScanStandard.Models.CZPallet;

namespace TrayScanStandard.Mediator.Handlers;

public class SaveScanLogCommandHandler(
    LinxContext context,
    ILogger<SaveScanLogCommandHandler> logger) : IRequestHandler<SaveScanLogCommand>
{
    public async Task Handle(SaveScanLogCommand request, CancellationToken cancellationToken)
    {
        var log = new PalletLog
        {
            ChannelCount = request.ChannelCount,
            Column = request.Column,
            ZuPanTime = request.ScanTime,
            PalletType = PalletType.组盘,
            BatteryInfo = Enumerable.Range(1, request.ChannelCount)
                .Select(index =>
                {
                    var code = request.Codes.GetValueOrDefault(index, string.Empty);
                    return new BatteryInfo
                    {
                        BatteryCode = code,
                        BatteryLevel = string.IsNullOrWhiteSpace(code) ? BatteryLevel.EMPTY : BatteryLevel.OK
                    };
                }).ToList()
        };

        context.PalletLogs.Add(log);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("扫码历史已保存: Id={Id}, 通道数量={ChannelCount}, 识别条码数={RealCount}",
            log.Id, log.ChannelCount, log.RealCount);
    }
}
