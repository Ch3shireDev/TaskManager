using System;
using CommandLine;
using TaskManagerLibrary;

namespace TaskManagerCLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Config config = null;
            Parser.Default.ParseArguments<Config>(args).WithParsed(x => { config = x; });
            if (config == null) return;
            var dbPath = config.DatabasePath;
            var database = Tools.LoadDatabase(dbPath);
            if (config.UpdateDatabase)
            {
                database.Update();
                Tools.SaveDatabase(database, dbPath);
            }
            if (config.SendMail)
            {
                if (database.IsLastMonthSent()) return;
                Tools.SendMail(database);
            }
        }
    }
}