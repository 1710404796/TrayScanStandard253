using LinxUniverse.Algo.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 检测条码命令
    /// </summary>
    /// <param name="Params"></param>
    public record DetectCodeCommand(ImmutableArray<ROIDetectParam> Params): IRequest<Either<string, ImmutableArray<CodeDetectResult>>>;
}
