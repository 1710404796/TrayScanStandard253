using System.Windows;
using MediatR;

namespace TrayScanStandard.Mediator.Commands
{
    /// <summary>
    /// 报警弹窗提示
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Caption"></param>
    /// <param name="Button"></param>
    /// <param name="SaveLog"></param>
    public record WarningBoxCommand(string Message,
        string Caption = "报警提示", MessageBoxButton Button = MessageBoxButton.OK, bool SaveLog = true) : IRequest<MessageBoxResult>;

    /// <summary>
    /// 信息弹窗提示
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Caption"></param>
    /// <param name="Button"></param>
    public record InformationBoxCommand(string Message,
        string Caption = "提示", MessageBoxButton Button = MessageBoxButton.OK) : IRequest<MessageBoxResult>;

    /// <summary>
    /// 来自PLC的报警弹窗提示
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Caption"></param>
    /// <param name="Button"></param>
    /// <param name="SaveLog"></param>

    public record PlcDataWarningBoxCommand(string Message,
    string Caption = "报警提示", MessageBoxButton Button = MessageBoxButton.OK, bool SaveLog = true) : IRequest<MessageBoxResult>;

    /// <summary>
    /// 来自WCS的报警弹窗提示
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="Caption"></param>
    /// <param name="Button"></param>
    /// <param name="SaveLog"></param>
    /// <param name="OutStationNum"></param>

    public record WcsDataWarningBoxCommand(string Message,
        string Caption = "报警提示", MessageBoxButton Button = MessageBoxButton.OK, bool SaveLog = true, int OutStationNum = 0) : IRequest<MessageBoxResult>;
}