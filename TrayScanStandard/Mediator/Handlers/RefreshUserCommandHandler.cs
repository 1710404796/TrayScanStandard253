using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinxUniverse.Auth;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using TrayScanStandard.Mediator.Commands;

namespace TrayScanStandard.Mediator.Handlers
{
    /// <summary>
    /// 刷新用户状态命令
    /// </summary>
    /// <param name="authenticationStateProvider"></param>
    internal class RefreshUserCommandHandler(LinxAuthenticationStateProvider authenticationStateProvider) : IRequestHandler<RefreshUserCommand>
    {

        public Task Handle(RefreshUserCommand request, CancellationToken cancellationToken)
        {
            (authenticationStateProvider as StandLinxAuthenticationStateProvider<LinxUser>).TestFlag = true;
            return Task.CompletedTask;
        }
    }
}
