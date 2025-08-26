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
using VMWebAIClient;
using LinxUniverse.Utils;
using System.IO;
namespace TrayScanStandard.Mediator.Handlers
{
    public class DetectCodeCommandHandler(
        IVMWebAIClient vMWebAIClient
        ) : IRequestHandler<DetectCodeCommand, Either<string, ImmutableArray<CodeDetectResult>>>
    {
        public Task<Either<string, ImmutableArray<CodeDetectResult>>> Handle(DetectCodeCommand request, CancellationToken cancellationToken)
        {

            var data = request.Params.Select((p, i) =>
            {
                var path = $"{FilenameHelper.AppPath}Data2D\\Detect-{FilenameHelper.FileName}-{i}.png";
                File.WriteAllBytes(path, p.ImageByte);
                return vMWebAIClient.DetectCodesV1Async(path, p.ROIS, cancellationToken);
            }
            //vMWebAIClient.DetectCodesV1Async(p.ImagePath, p.ROIs, cancellationToken)
            )
                .TraverseSerial(s => s)
                
            .Map(s => s.Traverse(s => s).Map(s => s.ToImmutableArray()));
            return data;
        }
    }
}
