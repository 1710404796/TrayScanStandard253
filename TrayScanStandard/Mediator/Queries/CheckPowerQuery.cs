using LinxUniverse.Mediator;
using MediatR;
using TrayScanStandard.Attritubes;

namespace TrayScanStandard.Mediator.Queries
{
    /// <summary>
    /// 检查权限查询
    /// </summary>
    /// <param name="PowerEnum"></param>
    [NotLog]
    public record CheckPowerQuery(PowerEnum PowerEnum) : IRequest<bool>;
}