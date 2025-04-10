using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace TrayScanStandard.Models.CZPallet
{
    public record GrabInfo(XYLStation From, XYLStation To, Arr<BatteryMoveInfo> Infos);
    public record BatteryMoveInfo(string Code, int FromChannel, int ToChannel);

    //public record GrabInfo
    //    (
    //    Option<string> ContainerFrom,
    //    Option<string> ContainerTo,
    //    string StationFrom,
    //    string StationTo,
    //    Lst<MaterialInfo> MaterialInfos
    //    );

    //public record MaterialInfo
    //(
    //    int ChannelFrom,
    //    int ChannelTo,
    //    string MaterialCode
    //    );
}
