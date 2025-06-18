using LinxUniverse.Algo.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Data.Models;

namespace TrayScanStandard.Mediator.Commands
{
    public record SaveDataFrameCommand(DataFrame DataFrame) : IRequest;

    public record DataFrame(CamImages[] Images, BatteryTypeInfo batteryTypeInfo, CodeInfo[] CodeInfos);
    public record CamImages(string Serial, ImageInfo[] ImagesPath);
    public record ImageInfo(string Path, int Exposure);
}
