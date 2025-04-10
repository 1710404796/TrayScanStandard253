using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace TrayScanStandard.Mediator.Commands.CZP
{
    public record MoveBatteryCommand(string stationFrom, string stationTo, int channel1, int channel2, string? checkCode = null) : IRequest<bool>;
}
