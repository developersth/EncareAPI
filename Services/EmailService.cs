using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    private readonly string _senderName;


    public EmailService(IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("EmailSettings");
        _smtpServer = emailSettings.GetValue<string>("SmtpServer");
        _smtpPort = emailSettings.GetValue<int>("SmtpPort");
        _smtpUser = emailSettings.GetValue<string>("SmtpUser");
        _smtpPassword = emailSettings.GetValue<string>("SmtpPassword");
        _senderName = emailSettings.GetValue<string>("SenderName");

    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_senderName, _smtpUser));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new SmtpClient();
        //await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.None);
        await client.AuthenticateAsync(_smtpUser, _smtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
