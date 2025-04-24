using LinxUniverse.Algo.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.Mediator.Commands
{
    public record DetectCodeCommand: IRequest<CodeDetectResult>;
}
