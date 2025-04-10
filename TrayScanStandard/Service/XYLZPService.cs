using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace TrayScanStandard.Service
{
    public class XYLZPService(WcsService wcsService, BatteryCacheService batteryCacheService, IMediator mediator)
    {
        public WcsService WcsService { get; } = wcsService;
        public BatteryCacheService BatteryCacheService { get; } = batteryCacheService;
        public IMediator Mediator { get; } = mediator;
    }
}
