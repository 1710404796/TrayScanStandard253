using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 推送图片命令
    /// </summary>
    /// <param name="Imgs"></param>
    public record PushImgCommand(string[] Imgs) : IRequest;
}
