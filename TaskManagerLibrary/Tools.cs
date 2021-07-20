using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TaskManagerLibrary
{
    public static class Tools
    {
        public static void SaveDatabase(Database database, string filepath)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            var data = serializer.Serialize(database);
            File.WriteAllText(filepath, data);
        }

        public static Database LoadDatabase(string filepath)
        {
            var data = File.ReadAllText(filepath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            return deserializer.Deserialize<Database>(data);
        }

        public static TimeOfDay AsTimeOfDay(this string time)
        {
            return TimeOfDay.Parse(time);
        }

        public static IEnumerable<string> GetProjectData(Database database)
        {
            return database.GetProjectData();
        }

        public static IEnumerable<string> GetProjectData(string filepath)
        {
            var database = LoadDatabase(filepath);
            return GetProjectData(database);
        }

        public static string GetCsv(string filepath)
        {
            var database = LoadDatabase(filepath);
            return GetCsv(database);
        }

        public static string GetCsv(Database database)
        {
            return database.GetCsv();
        }


        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static void SendMail(Connection connection, Database database,  string csv)
        {
            var date = DateTime.Now;

            var dateday = $"{date:yyyy-MM-dd} ({date.ToString("dddd", new CultureInfo("pl-PL"))})";
            var message = new MimeMessage
            {
                Subject = connection.Subject.Replace("<dateday>", dateday),
                From = {MailboxAddress.Parse(connection.MailFrom)}
            };

            foreach (var mail in connection.MailTo) message.To.Add(MailboxAddress.Parse(mail));

            var list = GetProjectData(database).ToArray();
            var data = string.Join("\n", list);

            var bodyBuilder = new BodyBuilder
            {
                TextBody = connection.TextBody.Replace("<dateday>", dateday).Replace("<data>", data)
            };

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

        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm");
        }

        public static Connection LoadConnection(string filepath)
        {
            var data = File.ReadAllText(filepath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            return deserializer.Deserialize<Connection>(data);
        }
    }
}