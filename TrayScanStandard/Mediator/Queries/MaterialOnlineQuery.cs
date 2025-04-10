using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace TrayScanStandard.Mediator.Queries
{
    public record MaterialOnlineQuery(ImmutableArray<Channel> Channels)
        : IRequest<Arr<Channel>>;


}
