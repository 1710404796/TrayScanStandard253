using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinxUniverse.Mediator;
using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 更新用户状态命令
    /// </summary>
    [NotLog]
    public record RefreshUserCommand : IRequest;
}
