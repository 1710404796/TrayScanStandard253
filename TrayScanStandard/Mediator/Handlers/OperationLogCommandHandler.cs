using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinxUniverse.Auth;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Mediator.Commands;

namespace TrayScanStandard.Mediator.Handlers
{
    internal class OperationLogCommandHandler(ILogger<OperationLogCommand> logger, LinxAuthenticationStateProvider authenticationStateProvider) : IRequestHandler<OperationLogCommand>
    {
        public async Task Handle(OperationLogCommand request, CancellationToken cancellationToken)
        {
            var user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user == null)
            {
                logger.LogWarning("Operator: Unknown Log: {msg}", request.Message);
                return;
            }
            if (user?.Identity?.IsAuthenticated ?? false)
            {
                logger.LogInformation("Operator: {usr}) Log: {msg}", user.GetUserName() + user.GetUserRole(), request.Message);
            }
            else
            {
                logger.LogWarning("Operator: Unknown Log: {msg}", request.Message);
            }
        }
    }
}
