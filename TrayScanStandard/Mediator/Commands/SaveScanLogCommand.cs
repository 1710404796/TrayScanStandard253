using MediatR;

namespace TrayScanStandard.Mediator.Commands;

public record SaveScanLogCommand(
    int ChannelCount,
    int Column,
    DateTime ScanTime,
    IReadOnlyDictionary<int, string> Codes) : IRequest;
