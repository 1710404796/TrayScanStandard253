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
    public record DetectCodeCommand(ImmutableArray<ROIDetectParam> Params): IRequest<Either<string, ImmutableArray<CodeDetectResult>>>;
}
