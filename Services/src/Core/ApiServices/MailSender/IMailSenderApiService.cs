namespace KubeFood.Core.ApiServices.MailSender;

public interface IMailSenderApiService
{
    Task<string> SendMailAsync(MailSenderModel model, CancellationToken cancellationToken = default);
}