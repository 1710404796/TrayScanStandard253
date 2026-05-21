using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayScanStandard.Service
{
    public interface IWcsResponse
    {
        /// <summary>
        /// 返回状态  0：成功  !0：异常
        /// </summary>
        int ResponseCode { get; }
        /// <summary>
        /// 描述信息
        /// </summary>
        string? ResponseMessage { get; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        string? Parameters { get; }
    }
}
