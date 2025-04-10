
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.UnitsOfMeasure;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Mediator.Queries;
using TrayScanStandard.Models;
using TrayScanStandard.Service;
using TrayScanStandard.Service.Models;
using TrayScanStandard.Utils;

namespace TrayScanStandard.Mediator.Handlers.WCS
{
    public class MaterialOnlineQueryHandler : IRequestHandler<MaterialOnlineQuery, Arr<Channel>>
    {
        private readonly IMediator _mediator;
        private readonly XYLZPService _xYLZPService;
        private readonly Func<MaterialOnlineRequest, Task<Result<MaterialOnlineResponse>>> _func;

        public MaterialOnlineQueryHandler(IMediator mediator, XYLZPService xYLZPService)
        {
            _mediator = mediator;
            _xYLZPService = xYLZPService;
            _func = WcsService.MakeApi<MaterialOnlineRequest, Result<MaterialOnlineResponse>>(
                xYLZPService.WcsService.MaterialOnlineAsync,
                data => new MaterialOnlineResponse()
                {
                    Materials = data.Materials.Map(m => new MaterialOnlineReturnInfo
                    {
                        MaterialCode = m.MaterialCode,
                        MaterialType = "dani",
                        StatusCode = m.MaterialCode switch
                        {
                            string x when x.IsNG() => 2,
                            _ => 1
                        }

                    }).ToList()
                },
                () => MainStorage.Saves.ApiEnableTable.CheckMaterialEnable
                );
        }
        public async Task<Arr<Channel>> Handle(MaterialOnlineQuery request, CancellationToken cancellationToken)
        {
            // 先调用MES获取拆组盘信息
            // 再调用MES高温静置入站











            var mRequest = new MaterialOnlineRequest()
            {
                Materials = [.. request.Channels
                        .Map(s => s.Battery.Match(Some: b => Some((s.ChannelId, b.Code)), None: None ))
                        .Choose(s => s)
                        //.Filter(s => BatteryLevelHelper.IsNG(s.Code))
                    
                 .Map(c =>
                 new MaterialOnlineInfo
                 { MaterialCode = c.Code, Channel = c.ChannelId.ToString()
                 })]
            };
            async Task<Map<string, BatteryLevel>> getCodeMap(MaterialOnlineRequest request)
            {
                if (request.Materials.Count <= 0)
                {
                    return new Map<string, BatteryLevel>();
                }
                var res = await _func(mRequest);
                var cmap = res.Match(
                    Succ: s => s.Materials.Map(r =>
                    (r.MaterialCode ?? "", BatteryLevelHelper.GetBatteryLevelFromWcs(r.StatusCode?.ToString() ?? "")

                    )
                    ),
                Fail: f =>
                {

                    _mediator.Send(new WcsDataWarningBoxCommand(f.Message)).Wait();
                    throw f;
                }


                ).DistinctBy(s => s.Item1).ToMap();

                return cmap;
            }

            var codeMap = await getCodeMap(mRequest);


            var data = request.Channels.Map(c =>
            c with
            {
                Battery = c.Battery.Map
                (b =>
                new Battery(
                    b.Code,
                    b.Code switch
                    {
                        var x when BatteryLevelHelper.IsNG(x) => BatteryLevel.NG,
                        _ => codeMap[b.Code]
                    }
                ))
            }

            ).ToArr();



            return data;
        }

    }
}

