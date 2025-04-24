using LinxUniverse.Algo.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Mediator.Commands;
using MugenCodeDetecter;
using System.Collections.Immutable;
namespace TrayScanStandard.Mediator.Handlers
{
    public class DetectCodeCommandHandler : IRequestHandler<DetectCodeCommand, Either<string, ImmutableArray<CodeDetectResult>>>
    {
        public Task<Either<string, ImmutableArray<CodeDetectResult>>> Handle(DetectCodeCommand request, CancellationToken cancellationToken)
        {
            var data = request.Params.Select(p =>
                MainStorage.Algo.Bind(s => s.DetectCodes(p))


            ).Traverse(s => s).Map(s => s.ToImmutableArray());
            return Task.FromResult(data);
        }
    }
}
