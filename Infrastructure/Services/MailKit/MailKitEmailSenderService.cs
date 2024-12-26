using Infrastructure.Data.MailKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

using MailKit.Net.Smtp;

using ApplicationCore.Interfaces;
using ApplicationCore.Dtos.Mail;


namespace Infrastructure.Services.MailKit
{
    public class MailKitEmailSenderService : IEmailSenderService
    {
        private readonly MailServerSettings _mailServerSettings;
        private readonly ILogger<MailKitEmailSenderService> _logger;

        public MailKitEmailSenderService(IOptions<MailServerSettings> options, ILogger<MailKitEmailSenderService> logger)
        {
            _mailServerSettings = options?.Value ?? throw new ArgumentException(nameof(options), "MailServerSettings is required.");
            _logger = logger;
        }

        public async Task SendEmailAsync(SendEmailRequest request)
        {
            //沒有填寄件人就用appsetting裡預設的人，預設Stellar
            var sender = request.SenderName ?? _mailServerSettings.DefaultSenderName;
            var senderEmail = _mailServerSettings.OfficialEmail;
            //參數(寄件誰,寄件信箱)
            var senderMailBoxAddress = new MailboxAddress(sender, senderEmail);

            //建立信件(寄件人、寄件位置)
            var message = new MimeMessage();
            //FROM=>所以是寄件人的資訊
            message.From.Add(senderMailBoxAddress);

            //收件人設定
            var receiverEmailAddress = request.ReceiverEmail;
            var receiver = request.Receiver;
            //
            message.To.Add(new MailboxAddress(receiver, receiverEmailAddress));


            message.Subject = request.Subject;


            var bodyBuilder = new BodyBuilder()
            {
                HtmlBody = request.Body,
            };
            message.Body = bodyBuilder.ToMessageBody();

            //寄出去
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_mailServerSettings.Host, _mailServerSettings.Port, _mailServerSettings.UseSsl);
                var officialEmail=_mailServerSettings.OfficialEmail;
                var password=_mailServerSettings.Password;
                await client.AuthenticateAsync(officialEmail, password);
                //寄送結果 寫入log
                var sendResult= await client.SendAsync(message);
                _logger.LogInformation($"SendResult:{sendResult}");
                await client.DisconnectAsync(true);
            }
        }
    }
}
