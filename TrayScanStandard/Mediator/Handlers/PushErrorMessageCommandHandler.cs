using MediatR;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.ViewModel;


namespace TrayScanStandard.Mediator.Handlers
{
    /// <summary>
    /// 推送错误消息命令处理程序
    /// </summary>
    /// <param name="viewModel"></param>
    public class PushErrorMessageCommandHandler(MainViewModel viewModel) : IRequestHandler<PushErrorMessageCommand>
    {
        public Task Handle(PushErrorMessageCommand request,
            CancellationToken cancellationToken)
        {
            viewModel.ErrorText = request.Message;
            //if (string.IsNullOrEmpty(viewModel.ErrorText))
            //{
            //    viewModel.Status = true;
            //}
            //else
            //{
            //    viewModel.Status = false;
            //}
            return Task.CompletedTask;
        }
    }
}