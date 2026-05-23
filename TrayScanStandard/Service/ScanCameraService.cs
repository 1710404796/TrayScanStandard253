using Basler.Fs.NET;
using Camera.Fs.Common;
using HKCamera.Fs.NET;
using HKCamera.Fs.NET.Controls;
using LanguageExt;
using LinxUniverse.DI;
using MediatR;
using Microsoft.Extensions.Logging;
using MugenCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using TrayScanStandard.ViewModel;
using static MvCamCtrl.NET.MyCamera;
namespace TrayScanStandard.Service
{
    /// <summary>
    /// 相机服务，负责管理相机连接和状态监控
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    /// <param name="cacheService">缓存服务</param>
    public class ScanCameraService(IMediator mediator, 
        ILogger<ScanCameraService> logger, 
        CacheService cacheService)
    {
        // 新增说明：把巡检间隔抽成常量，便于统一配置与后续调优。
        private static readonly TimeSpan MonitorInterval = TimeSpan.FromSeconds(10);

        public Option<MugenCamera.MugenCamera>[] MugenCameras { get; set; } = [];
        private Thread _listenThread;
        private readonly object _initLock = new();
        public BcrBorderViewModel[] BcrBorderViewModels = [];
        public Image2DViewModel[] Image2DViewModels = [];

        /// <summary>
        /// 初始化相机服务
        /// </summary>
        public void Init()
        {
            // 读取相机配置，初始化相机连接  按 相机总数 截取配置
            var settings = MainStorage.Saves.ConnectAddresses.Take(MainStorage.Saves.CameraCount).ToArray();

            // 初始化相机连接，并记录结果   按配置逐个连接相机
            var cameras = settings.Map((i, setting) => InitCameraWithLog(setting, i + 1, "startup")).ToArray();

            // 保存连接结果
            // Either->Option，成功 Some(camera)，失败 None 存入 MugenCameras
            MugenCameras = [.. cameras.Map(camera => camera.ToOption())];

            // 初始化相机视图模型
            Image2DViewModels = [.. settings.Map((i, s) => new Image2DViewModel() {
                CameraSetting = s,
                CameraIdx = i + 1,
                Service = this
            })];

            // 初始化边框视图模型，绑定相机连接状态
            BcrBorderViewModels = [.. Image2DViewModels.Map(s => new BcrBorderViewModel() { Image2DViewModel = s })];
            MugenCameras.Iter((i, camera) => BcrBorderViewModels[i].IsConnect = camera.Match(Some: c => c.IsConnect(), None: () => false));

            int successCount = cameras.Count(c => c.IsRight);
            int failCount = cameras.Length - successCount;
            logger.LogInformation($"[相机初始化] 完成: 总数={cameras.Length}, 成功={successCount}, 失败={failCount}");

            EnsureMonitorThreadStarted();

            //MugenCameras = cameras.Match
        }

        private void EnsureMonitorThreadStarted()
        {
            // 原有逻辑说明：初始化完成后启动一个后台线程，周期性执行连接维护任务。
            // 新增说明：抽成独立方法，避免 Init() 里职责过多，也便于后续替换为 Task 模式。
            if (_listenThread == null || !_listenThread.IsAlive)
            {
                _listenThread = new Thread(Listen)
                {
                    IsBackground = true,
                    Name = "ScanCameraService.Listen"
                };
                _listenThread.Start();
            }
        }

        // 获取指定索引的相机实例
        public Option< MugenCamera.MugenCamera> GetMugen(int idx) => MugenCameras[idx - 1];
        
        private void TryDestroyCamera(Option<MugenCamera.MugenCamera> cameraOption, int cameraIdx, string stage)
        {
            cameraOption.Match(
                Some: cam =>
                {
                    cam.Destroy().Match(
                        Right: _ =>
                        {
                            logger.LogInformation($"相机[{cameraIdx}]资源释放成功，阶段: {stage}");
                            return 0;
                        },
                        Left: err =>
                        {
                            logger.LogWarning($"相机[{cameraIdx}]资源释放失败，阶段: {stage}，错误: {err}");
                            return 0;
                        });
                    return 0;
                },
                None: () => 0);
        }

        private static Either<string, MugenCamera.MugenCamera> InitCamera(TrayScanStandard.Models.CameraSetting s)
        {
            // 根据地址创建相机对象
            return s.CameraAddresses.Create()
                                .Bind(MugenCameraExtensions.Connect)        // 建立连接
                                .Bind(s =>
                                {
                                    // 设置采集触发方式（按相机类型）
                                    switch (s)
                                    {
                                        case HikVision:
                                           return s.SetControl(new AcquisitionControl
                                            {
                                                TriggerMode = MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON,
                                                TriggerSource = MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE
                                            });
                                        case HuaruiCam:
                                            return s.SetControl(new HuaRui.Fs.NET.Controls.AcquisitionControl
                                            {
                                                TriggerMode = HuaRui.Fs.NET.Controls.TriggerModel.On,
                                                TriggerSource = HuaRui.Fs.NET.Controls.TriggerSource.Software
                                            });
                                        case BaslerCam:
                                            return s.SetControl(new Basler.Fs.NET.Controls.AcquisitionControl
                                            {
                                                TriggerMode = Basler.Fs.NET.Controls.TriggerModel.On,
                                                TriggerSource = Basler.Fs.NET.Controls.TriggerSource.Software
                                            });
                                        default:
                                        return Right<string, MugenCamera.MugenCamera>(s);

                                    };

                                    //return s.SetControl(c);
                                })
                                .Bind(s => s.SetControl(new
                                {    
                                    // 设置一下心跳
                                    GevHeartbeatTimeout = (long?)5000
                                }));
        }

        /// <summary>
        /// 单相机重连
        /// </summary>
        /// <param name="cameraIdx"></param>
        /// <returns></returns>
        public Either<string, bool> ReconnectCamera(int cameraIdx)
        {
            lock (_initLock)
            {
                if (cameraIdx <= 0 || cameraIdx > MainStorage.Saves.CameraCount)
                {
                    return Left($"相机索引越界: {cameraIdx}");
                }

                int arrayIdx = cameraIdx - 1;

                var setting = MainStorage.Saves.ConnectAddresses[arrayIdx];
                if (arrayIdx < MugenCameras.Length)
                {
                    TryDestroyCamera(MugenCameras[arrayIdx], cameraIdx, "manual_reconnect_cleanup");
                }

                var result = InitCameraWithLog(setting, cameraIdx, "manual_reconnect");
                return result.Match(
                    Right: camera =>
                    {
                        if (arrayIdx < MugenCameras.Length)
                        {
                            MugenCameras[arrayIdx] = Some(camera);
                        }

                        if (arrayIdx < BcrBorderViewModels.Length)
                        {
                            BcrBorderViewModels[arrayIdx].IsConnect = camera.IsConnect();
                        }

                        logger.LogInformation($"手动重连相机[{cameraIdx}]成功");
                        return Right<string, bool>(true);
                    },
                    Left: err =>
                    {
                        if (arrayIdx < MugenCameras.Length)
                        {
                            MugenCameras[arrayIdx] = Option<MugenCamera.MugenCamera>.None;
                        }

                        if (arrayIdx < BcrBorderViewModels.Length)
                        {
                            BcrBorderViewModels[arrayIdx].IsConnect = false;
                        }

                        logger.LogWarning($"手动重连相机[{cameraIdx}]失败: {err}");
                        return Left<string, bool>(err);
                    });
            }
        }

        /// <summary>
        ///  连接状态维护与自动重连
        /// </summary>
        void Listen()
        {
            while (!cacheService.Token.IsCancellationRequested)
            {
                try
                {
                    RunMonitorCycle();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "相机监听线程异常");
                }

                // 原有逻辑说明：每 10 秒巡检一次，收到取消信号时退出线程。
                if (cacheService.Token.WaitHandle.WaitOne(MonitorInterval))
                {
                    break;
                }
            }
        }

        private void RunMonitorCycle()
        {
            lock (_initLock)
            {
              
                // for 循环完成检查、重连和 UI 状态回写，减少数组分配和多次遍历。
                for (var i = 0; i < MugenCameras.Length; i++)
                {
                    var checkedCamera = MugenCameras[i]
                        .Bind(cam => cam.CheckConnect().ToOption());

                    var address = MainStorage.Saves.ConnectAddresses[i];
                    var finalCamera = checkedCamera.Match(
                        Some: cam =>
                        {
                            if (cam.IsConnect())
                            {
                                return Some(cam);
                            }

                            TryDestroyCamera(Some(cam), i + 1, "auto_reconnect_cleanup");
                            return InitCamera(address).ToOption();
                        },
                        None: () => InitCamera(address).ToOption());

                    MugenCameras[i] = finalCamera;

                    if (i < BcrBorderViewModels.Length)
                    {
                        BcrBorderViewModels[i].IsConnect = finalCamera.Match(
                            Some: cam => cam.IsConnect(),
                            None: () => false);
                    }
                }
            }
        }

        private Either<string, MugenCamera.MugenCamera> InitCameraWithLog(TrayScanStandard.Models.CameraSetting setting, int cameraIdx, string stage)
        {
            var addressText = GetAddressText(setting);
            //if (string.IsNullOrWhiteSpace(addressText))
            //{
            //    logger.LogWarning("相机[{CameraIdx}]连接参数为空，阶段: {Stage}", cameraIdx, stage);
            //}

            var result = InitCamera(setting);
            result.Match(
                Right: _ =>
                {
                    logger.LogInformation($"相机[{cameraIdx}]连接成功", string.IsNullOrWhiteSpace(addressText) ? "<empty>" : addressText);
                    return 0;
                },
                Left: error =>
                {
                    logger.LogWarning($"相机[{cameraIdx}]连接失败，错误: {error}", string.IsNullOrWhiteSpace(addressText) ? "<empty>" : addressText, error);
                    return 0;
                });

            return result;
        }


        private static string GetAddressText(TrayScanStandard.Models.CameraSetting setting)
        {
            return setting.CameraAddresses switch
            {
                HKAddress hk => hk.ConnectAddress switch
                {
                    Camera.Fs.Common.Key key => key.Value,
                    Camera.Fs.Common.IPAddress ip => ip.Value,
                    Camera.Fs.Common.Serial serial => serial.Value,
                    _ => string.Empty
                },
                HuaruiAddress hr => hr.ConnectAddress switch
                {
                    Camera.Fs.Common.Key key => key.Value,
                    Camera.Fs.Common.IPAddress ip => ip.Value,
                    Camera.Fs.Common.Serial serial => serial.Value,
                    _ => string.Empty
                },
                BaslerAddress basler => basler.ConnectAddress switch
                {
                    Camera.Fs.Common.Key key => key.Value,
                    Camera.Fs.Common.IPAddress ip => ip.Value,
                    Camera.Fs.Common.Serial serial => serial.Value,
                    _ => string.Empty
                },
                _ => string.Empty
            };
        }
    }
}
