using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
public class EmailService
{
    private readonly IConfiguration _configuration;

    static string[] Scopes = { GmailService.Scope.GmailSend };
    static string ApplicationName = "Encare Application";
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        UserCredential credential;
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json";
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true));
            Console.WriteLine("Credential file saved to: " + credPath);
        }

        // Create Gmail API service.
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        var emailMessage = new AE.Net.Mail.MailMessage
        {
            Subject = subject,
            Body = message,
            From = new MailAddress(_configuration["EmailSettings:SenderEmail"])
        };
        emailMessage.To.Add(new MailAddress(toEmail));
        emailMessage.ReplyTo.Add(emailMessage.From); // Bounces without this!!
        var msgStr = new StringWriter();
        emailMessage.Save(msgStr);

        var gmailMessage = new Message
        {
            Raw = Base64UrlEncode(msgStr.ToString())
        };

        await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
    }

    private static string Base64UrlEncode(string input)
    {
        var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(inputBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
}
