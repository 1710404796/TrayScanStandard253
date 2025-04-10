using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace TrayScanStandard.Mediator.Commands.CZP
{
    public record ScanPalletCodeCommand(int PalletIdx) : IRequest<string>;
}
