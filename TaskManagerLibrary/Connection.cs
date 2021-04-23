using System.Collections.Generic;

namespace TaskManagerLibrary
{
    public class Connection
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> MailTo { get; set; }
        public string MailFrom { get; set; }
        public string TextBody { get; set; }
        public string AttachmentName { get; set; }
    }
}