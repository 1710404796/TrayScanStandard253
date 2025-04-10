using System.Windows;
using LinxUniverse.Auth;
using MediatR;
using TrayScanStandard;
using TrayScanStandard.Mediator.Queries;


namespace TrayScanStandard.Mediator.Handlers
{
    /// <summary>
    /// 检查权限查询处理程序 // 根据需要去查询权限
    /// </summary>
    /// <param name="authenticationStateProvider"></param>
    public class CheckPowerQueryHandler(LinxAuthenticationStateProvider authenticationStateProvider) : IRequestHandler<CheckPowerQuery, bool>
    {
        public async Task<bool> Handle(CheckPowerQuery request, CancellationToken cancellationToken)
        {
            //var aa = LoginHelper.GetPowerId((await authenticationStateProvider.GetAuthenticationStateAsync()).User.GetUserRole()) - 1;
            //if (aa == 5 || (MainStorage.Saves.PowerTable.ContainsKey(request.PowerEnum) && 
            //                MainStorage.Saves.PowerTable[request.PowerEnum][aa]))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }
    }
}