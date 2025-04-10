using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 添加报警
    /// </summary>
    /// <param name="Message"></param>
    public record AddToWarningCommand(string Message) : IRequest;
}