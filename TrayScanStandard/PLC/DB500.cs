using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinxUniverse.PLC.Common.Interface;
using LinxUniverse.PLC.Common.Models;

namespace TrayScanStandard.PLC
{
    /// <summary>
    /// 托盘到位申请扫码数据块
    /// </summary>
    public class DB500 : DataStruct, ITaskCodeDelectDB
    {
        public DB500()
        {
            Db = 500;
            Length = 1;
        }

        [S7Offset(0)]
        public byte Action { get; set; }
    }
}
