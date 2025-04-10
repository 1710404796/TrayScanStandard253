using LinxUniverse.Auth;
using MediatR;
using TrayScanStandard.Mediator.Commands;

namespace TrayScanStandard.Mediator.Handlers
{
    /// <summary>
    /// 登出命令处理器
    /// </summary>
    /// <param name="authenticationStateProvider"></param>
    public class LogOutCommandHandler(LinxAuthenticationStateProvider authenticationStateProvider) : IRequestHandler<LogOutCommand>
    {
        public Task Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            (authenticationStateProvider as StandLinxAuthenticationStateProvider).ForceSignOut();
            return Task.CompletedTask;
        }
    }
}