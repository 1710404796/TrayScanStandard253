using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 初始化命令
    /// </summary>
    public record InitMeCommand : IRequest;
}