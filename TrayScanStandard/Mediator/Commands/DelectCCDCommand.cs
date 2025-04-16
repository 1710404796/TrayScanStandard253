using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Queries;

namespace TrayScanStandard.Mediator.Commands
{
    public record DelectResult(Arr<Channel> Channels, string[] Imgs);
    public record DelectCCDCommand(): IRequest<DelectResult>;
}
