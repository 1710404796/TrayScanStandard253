using LinxUniverse.Mediator;
using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 登录命令
    /// </summary>
    /// <param name="userId"></param>
    [NotLog]
    public record LoginCommand(string userId) : IRequest<bool>;

    /// <summary>
    /// 登出命令
    /// </summary>
    [NotLog]
    public record LogOutCommand() : IRequest;
}