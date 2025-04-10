using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 推送错误消息
    /// </summary>
    /// <param name="Message"></param>
    public record PushErrorMessageCommand(string Message) : IRequest;
}