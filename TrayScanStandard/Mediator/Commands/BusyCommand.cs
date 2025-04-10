using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 忙碌命令
    /// </summary>
    /// <param name="Value"></param>
    /// <param name="PValue"></param>
    /// <param name="message"></param>
    public record BusyCommand(bool Value, int PValue = 0, string message = "") : IRequest;
}