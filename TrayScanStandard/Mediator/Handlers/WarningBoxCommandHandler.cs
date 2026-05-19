using System.Windows;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Mediator.Commands;

namespace TrayScanStandard.Mediator.Handlers
{
    /// <summary>
    /// 弹窗警告命令处理器
    /// </summary>
    /// <param name="mediator"></param>
    public class WarningBoxCommandHandler(IMediator mediator) : IRequestHandler<WarningBoxCommand, MessageBoxResult>
    {
        public async Task<MessageBoxResult> Handle(WarningBoxCommand request, CancellationToken cancellationToken)
        {
            if (request.SaveLog)
                await mediator.Send(new AddToWarningCommand(request.Message));
            return MessageBox.Show(request.Message, request.Caption,
                request.Button,
                MessageBoxImage.Warning,
                MessageBoxResult.None,
                MessageBoxOptions.ServiceNotification);
        }
    }

    /// <summary>
    /// 弹窗信息命令处理器
    /// </summary>
    /// <param name="mediator"></param>
    public class InformationBoxCommandHandler(IMediator mediator) : IRequestHandler<InformationBoxCommand, MessageBoxResult>
    {
        public async Task<MessageBoxResult> Handle(InformationBoxCommand request, CancellationToken cancellationToken)
        {

            return MessageBox.Show(request.Message, request.Caption,
                request.Button,
                MessageBoxImage.Information,
                MessageBoxResult.None,
                MessageBoxOptions.ServiceNotification);
        }
    }

    /// <summary>
    /// 处理PLC数据校验返回的警告框
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    public class PlcDataWarningBoxCommandHandler(IMediator mediator, ILogger<PlcDataWarningBoxCommandHandler> logger) : IRequestHandler<PlcDataWarningBoxCommand, MessageBoxResult>
    {
        public Task<MessageBoxResult> Handle(PlcDataWarningBoxCommand request, CancellationToken cancellationToken)
        {
            return mediator.Send(new WarningBoxCommand($"PLC数据校验: \n{request.Message}", request.Caption, request.Button, request.SaveLog));

        }
    }

    /// <summary>
    /// 处理WCS返回的警告框
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    public class WcsDataWarningBoxCommandHandler(IMediator mediator, ILogger<WcsDataWarningBoxCommandHandler> logger) : IRequestHandler<WcsDataWarningBoxCommand, MessageBoxResult>
    {
        public Task<MessageBoxResult> Handle(WcsDataWarningBoxCommand request, CancellationToken cancellationToken)
        {
            return mediator.Send(new WarningBoxCommand($"WCS返回数据校验: \n{request.Message}", request.Caption, request.Button, request.SaveLog));
        }
    }
}
