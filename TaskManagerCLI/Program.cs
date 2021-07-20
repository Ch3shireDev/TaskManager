using System.IO;
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
            //if (config.UpdateDatabase)
            //{
            //    database.Update();
            //    Tools.SaveDatabase(database, dbPath);
            //}
            //if (config.SendMail)
            //{
            ////if (database.IsLastMonthSent()) return;
            var csv = Tools.GetCsv(database);

            File.WriteAllText("report.csv", csv);

            //var connection = Tools.LoadConnection(config.ConnectionPath);
            //Tools.SendMail(connection, database, csv);
            //}
        }
    }
}