using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LinxUniverse.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;


namespace TrayScanStandard.Mediator.Behaviors
{
    /// <summary>
    /// 日志行为
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var attr = request.GetType().GetCustomAttribute<NotLogAttribute>();
            bool logEnable = attr == null;
            try
            {

                if (logEnable)
                {
                    _logger.LogInformation($"正在执行: {typeof(TRequest).Name}");
                    _logger.LogDebug("请求数据: {data}", request);
                }

                var response = await next();
                if (logEnable)
                {
                    _logger.LogDebug("回复数据: {data}", response);

                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理请求发生错误: {Request}", typeof(TRequest).Name);
                throw;
            }
        }
    }
}
