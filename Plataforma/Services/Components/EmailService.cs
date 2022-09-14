using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Collections.Generic;

namespace Plataforma.Services.Components;

public class EmailService {
    private readonly ConfigurationsService _configurationsService;

    public EmailService(ConfigurationsService configurationsService) {
        _configurationsService = configurationsService;
    }

    public bool SendEmail(string to, string subject, string message) {
        return SendEmail(new[] { to }, subject, message);
    }

    public bool SendEmail(IEnumerable<string> to, string subject, string message) {
        try {
            using var smtpClient = new SmtpClient();
            smtpClient.Connect(_configurationsService.Smtp.Host, _configurationsService.Smtp.Port,
                _configurationsService.Smtp.Ssl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            smtpClient.Authenticate(_configurationsService.Smtp.Username, _configurationsService.Smtp.Password);

            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(_configurationsService.Title, _configurationsService.Smtp.Username));
            foreach (var email in to) mime.To.Add(new MailboxAddress(email, email));

            mime.Subject = subject;
            mime.Body = new TextPart("html") { Text = message };
            smtpClient.Send(mime);
            return true;
        } catch {
            return false;
        }
    }
}