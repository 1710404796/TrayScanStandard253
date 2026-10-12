using LinxUniverse.PLC.Common.Interface;
using LinxUniverse.PLC.Common.Models;
using LinxUniverse.PLC.Meditor.Queries;
using LinxUniverse.PLCProtos;
using MediatR;
using Microsoft.Extensions.Logging;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.PLC
{
    public class CCDContext(IMediator mediator, ILogger<CCDContext> logger) : IPLCContext
    {
        public string PlcGuid { get; set; }
        public Plc Plc { get; set; }

        public ITaskCodeDelectDB CodeDelectDB => throw new NotImplementedException();

        public ITaskCodeFeedbackDB CodeFeedbackDB => throw new NotImplementedException();

        public ITaskCodeDelectDB PCCodeDelectDB => throw new NotImplementedException();

        public ITaskCodeFeedbackDB PCCodeFeedbackDB => throw new NotImplementedException();


        public DB500 DB500 { get; set; } = new ();

        public DB501 DB501 { get; set; } = new ();

        public IMediator Mediator { get; } = mediator;

        public ILogger<CCDContext> Logger { get; } = logger;

        private byte? _lastAction;

        public ITaskCodeDelectDB GetCodeDelectDB(int idx)
        {
            ApplyAddressSettings();
            return DB500;
        }

        public ITaskCodeFeedbackDB GetCodeFeedbackDB(int idx)
        {
            ApplyAddressSettings();
            return DB501;
        }
        private void ApplyAddressSettings()
        {
            DB500.Db = MainStorage.Saves.PlcWriteDbBase;
            DB501.Db = MainStorage.Saves.PlcWriteDbBase + 1;
        }


        public Task Loop()
        {
            return Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateData()
        {
            try
            {
                var dbBase = MainStorage.Saves.PlcWriteDbBase;
                var offset = MainStorage.Saves.PlcWriteByteOffset;
                if (dbBase < 1 || dbBase > 65534 || offset < 0 || offset > 65535)
                {
                    Logger.LogWarning("PLC DB地址或字节偏移配置无效");
                    MainStorage.SetPlcConnectionStatus(false);
                    return false;
                }

                DB500.Db = dbBase;
                DB501.Db = dbBase + 1;

                var db500Data = await Mediator.Send(new S7ReadBytesQuery(PlcGuid, AreaType.DataBlock, dbBase, offset, 1));
                var db500Read = db500Data.Length == 1;
                if (!db500Read)
                {
                    Logger.LogWarning($"PLC报文读取失败: DB{dbBase}");
                    MainStorage.SetPlcConnectionStatus(false);
                    return false;
                }

                DB500.Action = db500Data[0];
                LogPlcTaskStart();

                var db501Data = await Mediator.Send(new S7ReadBytesQuery(PlcGuid, AreaType.DataBlock, dbBase + 1, offset, 1));
                var db501Read = db501Data.Length == 1;
                if (!db501Read)
                {
                    Logger.LogWarning($"PLC报文读取失败: DB{dbBase + 1}");
                    MainStorage.SetPlcConnectionStatus(false);
                    return false;
                }

                DB501.ToFeedback = db501Data[0];
                MainStorage.SetPlcConnectionStatus(db501Read);
                return db501Read;
            }
            catch
            {
                MainStorage.SetPlcConnectionStatus(false);
                throw;
            }
        }

        private void LogPlcTaskStart()
        {
            var action = DB500.Action;
            if (_lastAction == action)
                return;

            _lastAction = action;
            if (action == 0)
                return;

            var taskName = action switch
            {
                1 => "扫码",
                2 => "托盘离开",
                _ => "未知任务"
            };

            Logger.LogInformation($"PLC发起任务: Action={action}, TaskName={taskName}");
        }
    }
}
