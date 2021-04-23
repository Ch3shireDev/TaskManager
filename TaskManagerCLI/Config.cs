using CommandLine;

namespace TaskManagerCLI
{
    public class Config
    {
        [Option('d', "database", HelpText = "Path for YAML database.", Required = true)]
        public string DatabasePath { get; set; }

        [Option('s', "send-mail", HelpText = "Send mail to recipients?", Required = false)]
        public bool SendMail { get; set; }

        [Option('u', "update", HelpText = "Update database?", Required = false)]
        public bool UpdateDatabase { get; set; }
    }
}