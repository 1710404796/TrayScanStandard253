using LinxUniverse.Algo.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrayScanStandard.Data.Models;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 保存数据框命令
    /// </summary>
    /// <param name="DataFrame"></param>
    public record SaveDataFrameCommand(DataFrame DataFrame) : IRequest;

    /// <summary>
    /// 数据帧
    /// </summary>
    /// <param name="Images">图片</param>
    /// <param name="batteryTypeInfo">电池类型信息</param>
    /// <param name="CodeInfos">条码信息</param>
    public record DataFrame(CamImages[] Images, BatteryTypeInfo batteryTypeInfo, CodeInfo[] CodeInfos);

    /// <summary>
    /// 摄像头图像
    /// </summary>
    /// <param name="Serial">序列号</param>
    /// <param name="ImagesPath">图片路径</param>
    public record CamImages(string Serial, ImageInfo[] ImagesPath);

    /// <summary>
    /// 图片信息
    /// </summary>
    /// <param name="Path">图片路径</param>
    /// <param name="Exposure">曝光时间</param>
    public record ImageInfo(string Path, int Exposure);
}
