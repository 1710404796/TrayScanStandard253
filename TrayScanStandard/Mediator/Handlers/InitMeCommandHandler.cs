
using Humanizer;
using LinxUniverse.Auth;
using LinxUniverse.CST;
using LinxUniverse.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using RPDelectPallet.Meditor.Queries;
using System.Data;
using TrayScanStandard.Attritubes;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Service;

namespace TrayScanStandard.Mediator.Handlers
{
    /// <summary>
    /// 初始化命令处理器
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    /// <param name="role"></param>
    public class InitMeCommandHandler(IMediator mediator,
        ILogger<InitMeCommandHandler> logger, 
        RoleManager<LinxRole, LinxUser> role
        , ScanCameraService scanCameraService
        )
        : IRequestHandler<InitMeCommand>
    {
        public async Task Handle(InitMeCommand request, CancellationToken cancellationToken)
        {
            FilenameHelper.CreateDir("Data2D");

            foreach (var item in Enum.GetValues<PowerEnum>())
            {
                if (!MainStorage.Saves.PowerTable.ContainsKey(item))
                {
                    MainStorage.Saves.PowerTable[item] = [];
                }

                foreach (var item1 in Enum.GetValues<RoleEnum>())
                {
                    if (!MainStorage.Saves.PowerTable[item].ContainsKey(item1))
                    {
                        MainStorage.Saves.PowerTable[item][item1] = false;
                    }
                }
            }


            foreach (var item in Enum.GetNames<RoleEnum>())
            {

                await role.CreateAsync(new LinxRole { RoleName = item });
            }
            scanCameraService.Init();
            // 通常这里要初始化一些硬件设备

            // 例如：相机、光源、CST等

            // 也需要注册一些任务等
            MainStorage.CST = await MainStorage.Saves.LightInfos.Map(
                async s =>
                {
                    var com = s.Com.DehumanizeTo<SerialPortType>();
                    var g = await mediator.Send(new CreateCSTLightCommand(Com: com));
                    return await mediator.Send(new GetLightQuery(g));
                }).TraverseSerial(s => s!);
            //messageboxmanager
            //MainStorage.Cst[0] =
            //    await mediator.Send(new CreateCSTLightCommand(Com: SerialPortType.COM1));
            //if (string.IsNullOrEmpty(MainStorage.Saves.ExtCamKey))
            //{
            //    logger.LogWarning("广角镜头序列码未设定");
            //    return;
            //}
            //MainStorage.CamId = await mediator.Send(new CreateCameraCommand(new CameraDeciveInfo
            //{
            //    CameraType = CameraType.HK2d,
            //    Key = MainStorage.Saves.ExtCamKey,

            //}));

            //if (MainStorage.CamId != null)
            //{
            //    var cam = await mediator.Send(new GetCameraQuery(MainStorage.CamId)) as HKGigeTriggerCamera;
            //    cam.InitCamera();
            //    // todo: 曝光是否要标准化
            //    cam.SetExposureTime(MainStorage.Saves.ExtCamExposure);
            //    MainStorage.Cam = cam; 
            //}
            //else
            //{
            //    await mediator.Send(new WarningBoxCommand("广角相机初始化失败", SaveLog: false));
            //}
            //MainStorage.Cst[1] =
            //await mediator.Send(new CreateCSTLightCommand(Com: SerialPortType.COM2));

        }
    }
}