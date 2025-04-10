using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Mediator.Commands.Wcs;
using TrayScanStandard.Mediator.Queries;
using TrayScanStandard.Models;
using TrayScanStandard.Service;


namespace TrayScanStandard.Mediator.Handlers.WCS
{
    internal class CheckMaterialOnlineCommandHandler(XYLZPService xylZPService, ILogger<GrabActionCommandHandler> logger) : IRequestHandler<CheckMaterialOnlineCommand, Arr<Channel>>
    {
        public async Task<Arr<Channel>> Handle(CheckMaterialOnlineCommand request, CancellationToken cancellationToken)
        {
            if (MainStorage.Saves.TestMode)
            {
                //return request.MaterialInfos.Select(s => (s.Channel, BatteryLevel.OK)).ToArray();
                //return request.MaterialInfos.Select(s => (s.Channel, (BatteryLevel)Random.Shared.Next(1, 1))).ToArray();
                // request.MaterialInfos.Select(s => (s.Channel, s.MaterialCode.Contains("Noread", StringComparison.InvariantCultureIgnoreCase) ? BatteryLevel.NG : BatteryLevel.OK)).ToArray();
                //return request.MaterialInfos.Select(s => (s.Channel, (BatteryLevel)MainStorage.Saves.TestLevel)).ToArray();

                //将“-”电芯条码判定为NG

                return request.MaterialInfos.Select(s =>
                {
                    bool isNotZeroPrefixed = !s.MaterialCode.StartsWith("-", StringComparison.InvariantCultureIgnoreCase);
                    BatteryLevel batteryLevel = isNotZeroPrefixed || s.MaterialCode.Contains("Noread", StringComparison.InvariantCultureIgnoreCase)
                                                 ? BatteryLevel.NG
                                                 : BatteryLevel.OK;
                    return new Channel(s.Channel, new Battery(s.MaterialCode, batteryLevel));
                }).ToArr();


            }




            var data = await xylZPService.WcsService.MaterialOnlineAsync(new Service.Models.MaterialOnlineRequest
            {
                Materials = request.MaterialInfos.Select(s => new Service.Models.MaterialOnlineInfo
                {
                    Channel = (s.Channel + 1).ToString(),
                    MaterialCode = s.MaterialCode,
                }).ToList(),
            });

            if (data != null)
            {
                //(int, BatteryLevel)[] res = new (int, BatteryLevel)[data.Materials.Count];

                //var batterys = data.Materials.Select(material =>
                //{
                //    return (material.MaterialCode ?? string.Empty, material.StatusCode switch
                //    {
                //        0 => BatteryLevel.OK,
                //        1 => BatteryLevel.NG,
                //        _ => throw new Exception($"{Properties.Resources.UnknownBatteryCellGrade},{material.StatusCode}")
                //    });
                //}).DistinctBy(s => s.Item1).ToImmutableDictionary(s => s.Item1, s => s.Item2);

                //var res = request.MaterialInfos.Select(s =>
                //{
                //    var batteryLevel = batterys.TryGetValue(s.MaterialCode, out var level) ? level : BatteryLevel.NG;
                //    return new Channel(s.Channel, new Battery(s.MaterialCode, batteryLevel));
                //}).ToArr();








                //for (int i = 0; i < data.Materials.Count; i++)
                //{
                //    BatteryLevel batteryLevel = data.Materials[i].StatusCode switch
                //    {
                //        0 => BatteryLevel.OK,
                //        1 => BatteryLevel.NG,
                //        //"rework" => BatteryLevel.NG,
                //        //"7" => BatteryLevel.REWORK,
                //        //_ => throw new System.Exception($"未知的电芯等级{data.Materials[i].StatusCode}")
                //        _ => throw new Exception($"{Properties.Resources.UnknownBatteryCellGrade},{data.Materials[i].StatusCode}")
                //    };

                //    int? idx = request.MaterialInfos.FirstOrDefault(s => s.MaterialCode == data.Materials[i].MaterialCode)?.Channel;

                //    if (data.Materials[i]?.MaterialCode?.ToLower() == "noread")
                //    {
                //        idx = i;
                //    }
                //    if (idx == null)
                //    {
                //        //await xylZPService.Mediator.Send(new WcsDataWarningBoxCommand("校验条码返回不一致！请检查日志"));
                //        await xylZPService.Mediator.Send(new WcsDataWarningBoxCommand(Properties.Resources.InconsistentBarcodeVerificationReturned, Properties.Resources.PleaseCheckTheLogs));
                //        return default;


                //    }
                //res[i] = (idx.Value, batteryLevel);

                //lock (MainStorage.Saves.BatterysLvTable)
                //{
                //    MainStorage.Saves.BatterysLvTable.Add(new BatteryLv { Code = data.Materials[i]?.MaterialCode, Level = batteryLevel });

                //}



                //}
                throw new Exception("已经废弃的代码");


            }

            return default;
        }
    }
}
