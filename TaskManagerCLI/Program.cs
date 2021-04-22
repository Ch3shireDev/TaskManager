using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using TaskManagerLibrary;

namespace TaskManagerCLI
{
    internal class Program
    {
        private static readonly string dbPath = "../../../database.yml";

        private static void Main(string[] args)
        {
            var database = Tools.LoadDatabase(dbPath);

            var date = DateTime.Now;
            var connection = database.Connection;

            var list = Tools.GetProjectData(dbPath, DateTime.Now).ToArray();
            var dateday = $"{date:yyyy-MM-dd} ({date.ToString("dddd", new CultureInfo("pl-PL"))})";
            var message = new MimeMessage
            {
                Subject = connection.Subject.Replace("<dateday>", dateday),
                From = {MailboxAddress.Parse(connection.MailFrom)}
            };

            foreach (var mail in connection.MailTo) message.To.Add(MailboxAddress.Parse(mail));

            var data = string.Join("\n", list);

            var bodyBuilder = new BodyBuilder
            {
                TextBody = connection.TextBody.Replace("<dateday>", dateday).Replace("<data>", data)
            };

            var csv = Tools.GetCsv(dbPath);

            var reportCsv = connection.AttachmentName;
            if (File.Exists(reportCsv)) File.Delete(reportCsv);
            File.WriteAllText(reportCsv, csv, Encoding.UTF8);

            bodyBuilder.Attachments.Add(reportCsv, File.ReadAllBytes(reportCsv));

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(connection.Server, connection.Port);
                client.Authenticate(connection.Username, connection.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}