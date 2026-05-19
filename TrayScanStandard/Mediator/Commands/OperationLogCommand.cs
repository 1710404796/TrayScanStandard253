using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 操作日志
    /// </summary>
    /// <param name="Message"></param>
    public record OperationLogCommand(string Message) : IRequest;
}
