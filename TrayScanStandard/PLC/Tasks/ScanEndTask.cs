using LinxUniverse.PLC.Common.Models;
using LinxUniverse.PLCProtos;
using Microsoft.Extensions.Logging;

namespace TrayScanStandard.PLC.Tasks
{
    /// <summary>
    /// 托盘离开任务（PLC action=2）
    /// </summary>
    [S7TaskDb(500, 501)]
    public class ScanEndTask : CoreTask<CCDContext>
    {
        public ScanEndTask() : base(2)
        {
            TaskName = "托盘离开";
        }

        public override Task<bool> DoSth()
        {
            Logger?.LogInformation("收到托盘离开任务，准备结束本次扫码流程");
            return Task.FromResult(true);
        }

        protected override Task<bool> CleanData()
        {
            Logger?.LogInformation("托盘离开任务完成，流程复位");
            return Task.FromResult(true);
        }
    }
}
