
using Humanizer;
using LinxUniverse.Auth;
using LinxUniverse.CST;
using LinxUniverse.DI;
using LinxUniverse.PLCProtos;
using LinxUniverse.Utils;
using LinxUniverse.VM;
using MediatR;
using Microsoft.Extensions.Logging;
using MugenCodeDetecter;
using RPDelectPallet.Meditor.Queries;
using System.Data;
using System.Text;
using System.Windows;
using TrayScanStandard.Attritubes;
using TrayScanStandard.Data;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Models.CZPallet;
using TrayScanStandard.PLC;
using TrayScanStandard.PLC.Tasks;
using TrayScanStandard.Service;
using VMWebAIClient;

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
        RoleManager<LinxRole, LinxUser> role, 
        ScanCameraService scanCameraService, 
        LinxContext linxContext, 
        IVMWebAIClient vmWebAIClient,
        PLCTaskService<CCDContext> pLCTaskService,
        CacheService cacheService
        ) : IRequestHandler<InitMeCommand>
    {
        public async Task Handle(InitMeCommand request, CancellationToken cancellationToken)
        {
            FilenameHelper.CreateDir("DataFrame");
            FilenameHelper.CreateDir("InsertLog");
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

            // 让程序能够处理 GB2312、GBK 等中文编码（非 Unicode 编码）
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // 读取当前电池类型
            MainStorage.SelectBattery = linxContext.BatteryTypeInfos.FirstOrDefault(s => s.Id == MainStorage.Saves.SelectBatteryId);

            // 优先初始化相机，避免 PLC 初始化卡住导致相机页面无法显示。
            scanCameraService.Init();

            // PLC 初始化改为异步超时执行，不阻塞相机/页面初始化。
            _ = InitPlcSafelyAsync();

            foreach (var item in Enum.GetNames<RoleEnum>())
            {

                await role.CreateAsync(new LinxRole { RoleName = item });
            }


            // 通常这里要初始化一些硬件设备

            // 例如：相机、光源、CST等

            // 也需要注册一些任务等

            // 初始化光源
            var cstList = await MainStorage.Saves.LightInfos.Map(
                async s =>
                {
                    if (!Enum.TryParse<SerialPortType>(s.Com, true, out var com))
                    {
                        return null;
                    }

                    // 根据光源类型选择底层驱动：
                    // Cognex -> CSTControllerDll
                    // Wordop -> 串口 ASCII 协议（固定 19200）
                    var controllerType = s.Type == LightType.Wordop
                        ? LightControllerType.Wordop
                        : LightControllerType.Cognex;
                    var baudRate = s.Type == LightType.Wordop ? 19200 : 9600;

                    // 注意：LoggingBehavior 出错时会返回 default；这里必须显式判空，避免后续 NRE。
                    var guid = await mediator.Send(new CreateCSTLightCommand(
                        Com: com,
                        ControllerType: controllerType,
                        BaudRate: baudRate));
                    if (string.IsNullOrWhiteSpace(guid))
                    {
                        logger.LogError($"[光源连接] 失败，Type={s.Type}, Com={s.Com}, Baud={baudRate}");
                        return null;
                    }

                    var cst = await mediator.Send(new GetLightQuery(guid));
                    if (cst == null)
                    {
                        logger.LogError($"[光源连接] 获取实例失败，Type={s.Type}, Com={s.Com}, Guid={guid}");
                        return null;
                    }

                    logger.LogInformation($"[光源连接] 成功，Type={s.Type}, Com={s.Com}, Guid={guid}");
                    return cst;
                }).TraverseSerial(s => s);

            // 仅保留初始化成功的光源控制器，避免拍照流程访问到空对象。
            MainStorage.CST = cstList.Where(s => s != null).Select(s => s!);
            var lightSuccess = cstList.Count(s => s != null);
            var lightFail = MainStorage.Saves.LightInfos.Length - lightSuccess;
            logger.LogInformation($"[光源初始化] 完成，成功={lightSuccess}，失败={lightFail}");
            //var sol= CodeDetectExtensions
            //    .LoadSolution(new VMSolutionInfo(@"test.sol", ""));
            //Console.WriteLine(sol);
            //MainStorage.Algo = sol
            //    .Bind(s => s.CreateAlgo(new DetectVMConfig("test", "legacy_detect")))
            //    ;
            //MainStorage.AlgoCnn = sol
            //    .Bind(s => s.CreateAlgo(new DetectVMCnnConfig("test", "cnn_detect")));



            // 算法加载
            var algores = await vmWebAIClient.CreateAlgoAsync(FilenameHelper.AppPath + @"test.sol", LinxUniverse.Algo.Common.DetectType.VisionMaster);

            logger.LogInformation("算法加载结果: {Result}", algores);


            if (MainStorage.CST == null) 
            { 
                logger.LogError("光源初始化失败");
                MessageBox.Show("光源初始化失败", "光源初始化失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            algores.IfLeft(
                s =>
                {
                    logger.LogError(s);
                    MessageBox.Show("算法加载错误\n" + s, "算法加载错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            );
            MainStorage.Algo.IfLeft(
                s =>
                {
                    logger.LogError(s);
                    MessageBox.Show("算法加载错误\n" + s, "算法加载错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            );
            MainStorage.AlgoCnn.IfLeft(
              s =>
              {
                  logger.LogError(s);
                  MessageBox.Show("cnn算法加载错误\n" + s, "算法加载错误", MessageBoxButton.OK, MessageBoxImage.Error);
                  return;
              }
          );

            #region
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
            #endregion
        }

        private async Task InitPlcSafelyAsync()
        {
            const int plcInitTimeoutMs = 5000;
            try
            {
                var initTask = pLCTaskService.Init(
                    new LinxUniverse.PLC.Meditor.Commands.S7CreatePlcCommand(
                        S7.Net.CpuType.S71200,
                        MainStorage.Saves.PLCIP,
                        0,
                        1),
                    cacheService.Token);

                var completed = await Task.WhenAny(initTask, Task.Delay(plcInitTimeoutMs, cacheService.Token));
                if (completed != initTask)
                {
                    logger.LogWarning($"[PLC初始化] 超时({plcInitTimeoutMs}ms)，地址={MainStorage.Saves.PLCIP}，后续可手动重试");
                    return;
                }

                await initTask;
                pLCTaskService.RegisterTask<ScanTheCodeTask>();
                pLCTaskService.RegisterTask<ScanEndTask>();
                logger.LogInformation($"[PLC初始化] 完成，地址={MainStorage.Saves.PLCIP}");
            }
            catch (OperationCanceledException) when (cacheService.Token.IsCancellationRequested)
            {
                logger.LogInformation($"[PLC初始化] 已取消，地址={MainStorage.Saves.PLCIP}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[PLC初始化] 失败，地址={MainStorage.Saves.PLCIP}");
            }
        }
    }
}
