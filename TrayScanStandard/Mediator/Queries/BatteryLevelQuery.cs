
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Models;

namespace TrayScanStandard.Mediator.Queries
{
    public record BatteryLevelQuery(Pallet Pallet) : IRequest<Pallet>;
    // 直接返回level吧

    public record Pallet(string PalletCode, ImmutableArray<Channel> Channels, bool IsFake);
    // ChannelId从1开始
    public record Channel(int ChannelId, Option<Battery> Battery);
    //public record Channel(int ChannelId, Option<Battery> BattleWithLevel);

    public record Battery(string Code, BatteryLevel Level);
}
