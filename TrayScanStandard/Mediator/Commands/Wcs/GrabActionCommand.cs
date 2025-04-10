using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using MediatR;
using TrayScanStandard.Mediator.Queries;
using TrayScanStandard.Models;
using TrayScanStandard.Models.CZPallet;


namespace TrayScanStandard.Mediator.Commands.Wcs
{
    /// <summary>
    /// 4.1 拆组开始完成
    /// </summary>
    /// <param name="InStationNum">工位号</param>
    /// <param name="Start">是否是 开始信号</param>
    public record GrabActionCommand(int StationNum, bool Start) : IRequest<bool>;

    /// <summary>
    /// 4.4
    /// </summary>
    /// <param name="data"></param>
    public record GrabCommand(GrabInfo Data) : IRequest<bool>;

    /// <summary>
    /// 4.3
    /// </summary>
    /// <param name="InStationNum"></param>
    public record ApplyBatteryInfoCommand(int StationNum) : IRequest<BatteryLevel[]?>;

    // 这个key构思一下
    public record BattleLevelQuery(Queries.Pallet Pallet) : IRequest<Queries.Pallet>;
    /// <summary>
    /// 4.2
    /// </summary>
    /// <param name="MaterialInfos"></param>
    /// 
    [Obsolete]
    public record CheckMaterialOnlineCommand(MaterialInfo[] MaterialInfos) : IRequest<Arr<Channel>>;

    public record MaterialInfo(string MaterialCode, int Channel);


    public record AskFakeCellPalletCommand(int StationNum) : IRequest;
    public record MoveFakeCellPalletCommand(int StationNum) : IRequest<bool>;
    public record DeviceErrorCommand(ushort ErrorCode) : IRequest;






}
