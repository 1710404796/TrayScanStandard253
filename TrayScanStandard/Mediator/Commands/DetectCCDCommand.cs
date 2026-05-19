using LinxUniverse.Algo.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Data.Models;
using TrayScanStandard.Mediator.Queries;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 检测结果
    /// </summary>
    /// <param name="Channels"></param>
    public record DetectResult(Arr<CodeInfo> Channels);

    /// <summary>
    /// 检测CCD命令
    /// </summary>
    /// <param name="BatteryTypeInfo"></param>
    public record DetectCCDCommand(BatteryTypeInfo? BatteryTypeInfo): IRequest<Either<string, DetectResult>>;
}
