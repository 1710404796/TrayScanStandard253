using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;
using TrayScanStandard.Mediator.Commands;

namespace TrayScanStandard.Mediator.Handlers
{
    /// <summary>
    /// 添加报警处理程序
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public class AddToWarningCommandHandler(LinxContext context, ILogger<AddToWarningCommandHandler> logger) : IRequestHandler<AddToWarningCommand>
    {
        public async Task Handle(AddToWarningCommand request, CancellationToken cancellationToken)
        {
            context.WarningLogs.Add(new WarningLog
            {
                Message = request.Message
            });
            try
            {
                var rows = await context.SaveChangesAsync();
                if (rows == 0)
                {
                    logger.LogWarning("写入报警日志失败");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("写入报警发生错误 ex: {ex}", ex.Message);
            }

        }
    }
}