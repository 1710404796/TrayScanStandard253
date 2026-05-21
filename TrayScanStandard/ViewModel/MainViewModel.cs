using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinxUniverse.Auth;
using LinxUniverse.DI;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Models;
using TrayScanStandard.Service;

namespace TrayScanStandard.ViewModel
{
    public partial class MainViewModel : ObservableRecipient
    {
        public static WcsSaves Saves => MainStorage.Saves;

        [ObservableProperty]
        private Visibility _hasBackGround = Saves.BackGroundEnable;


        [ObservableProperty]
        Visibility _isLock = Visibility.Visible;

        [ObservableProperty]
        Visibility _isBusy = Visibility.Collapsed;

        [ObservableProperty] private int _progg = 0;
        [ObservableProperty] private string _busyMessage = string.Empty;
        [ObservableProperty] private string _errorText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLoading))]
        // 标准化一下
        private bool _isWcsEnable = false;
        private Thread _someThread;
        private volatile bool _shouldStopThread = false;

        public bool IsLoading => !IsWcsEnable;

        public IMediator Mediator { get; }
        public LinxAuthenticationStateProvider AuthenticationStateProvider { get; }
        public ILogger<MainViewModel> Logger { get; }
        [ObservableProperty] private string _username;


        [RelayCommand]
        public async Task SignOut()
        {
            Logger.LogInformation("{username} 登出", Username);

            // _authenticationStateProvider.
            await Mediator.Send(new LogOutCommand());
        }


        public string PlcStatus => PlcIsRunning ? Properties.Resources.Run : Properties.Resources.Stop;
        public SolidColorBrush PlcColor => PlcIsRunning ? Brushes.Green : Brushes.Red;

        [NotifyPropertyChangedFor(nameof(PlcStatus))]
        [NotifyPropertyChangedFor(nameof(PlcColor))]
        [ObservableProperty]
        private bool _plcIsRunning = false;

        public MainViewModel(IMediator mediator,
                LinxAuthenticationStateProvider authenticationStateProvider,
                ILogger<MainViewModel> logger, CacheService cacheService)
        {

            Task.Run(async () =>
            {
                bool initSuccess = false;
                try
                {
                    // 初始化命令
                    await mediator.Send(new InitMeCommand());
                    initSuccess = true;
                    //await Task.WhenAll(mediator.Send(new ChangeCstAllCommand(30)));

                    //await codeReaderService.Init();
                    Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    // 考虑这个位置
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "系统初始化失败,请逐一检查相机、光源、PLC、WCS连接状态");

                    // 兜底：若相机已初始化成功，则允许进入“可用”状态，避免页面被整体阻断。
                    try
                    {
                        var scanCameraService = App.GetService<ScanCameraService>();
                        if (scanCameraService.BcrBorderViewModels.Length > 0)
                        {
                            initSuccess = true;
                            logger.LogWarning("系统初始化部分失败，但相机模块已就绪，进入部分可用状态");
                        }
                    }
                    catch (Exception innerEx)
                    {
                        logger.LogError(innerEx, "初始化失败后检查相机就绪状态时发生错误");
                    }
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainStorage.IsWcsEnable = IsWcsEnable = initSuccess;
                    });
                }
            });
            authenticationStateProvider.AuthenticationStateChanged += task =>
            {
                // 需要根据类型嗼
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    System.Security.Claims.ClaimsPrincipal user = (await task).User;
                    IsLock = user.Identity.IsAuthenticated ? Visibility.Collapsed : Visibility.Visible;
                    if (user.Identity.IsAuthenticated)
                        Username = $"{user.GetUserName()}({user.GetUserRole()})";
                    logger.LogInformation("{username} 登录", Username);
                    // 输出切换后用户名称
                    // Console.WriteLine(IsLock);

                });
            };
            _someThread = new Thread(SomeLoop);
            _someThread.Start();
            Mediator = mediator;
            AuthenticationStateProvider = authenticationStateProvider;
            Logger = logger;
            CacheService = cacheService;
        }

        public CancellationTokenSource Maints = new CancellationTokenSource();
        public readonly CacheService CacheService;

        private async void SomeLoop(object? obj)
        {
            while (!IsWcsEnable && !_shouldStopThread)
            {
                await Task.Delay(1000);
            }
            while (!CacheService.Token.IsCancellationRequested && !_shouldStopThread)
            {
                //PlcIsRunning = XcplcService.IsPlcRunning;
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 用于停止后台线程并防止内存泄漏的清理方法
        /// </summary>
        public void Cleanup()
        {
            _shouldStopThread = true;

            if (_someThread != null && _someThread.IsAlive)
            {
                if (!_someThread.Join(TimeSpan.FromSeconds(2)))
                {
                    // 如果线程未能优雅停止，则显示日志警告。
                    Logger?.LogWarning("后台线程在超时后未正常终止");
                }
            }

            Maints?.Cancel();
            Maints?.Dispose();
        }
    }


}
