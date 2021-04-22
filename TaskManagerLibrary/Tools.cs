using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TaskManagerLibrary
{
    public static class Tools
    {
        public static void SaveDatabase(TaskScheduleDatabase database, string filepath)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            var data = serializer.Serialize(database);
            File.WriteAllText(filepath, data);
        }

        public static TaskScheduleDatabase LoadDatabase(string filepath)
        {
            var data = File.ReadAllText(filepath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            return deserializer.Deserialize<TaskScheduleDatabase>(data);
        }

        public static TimeOfDay AsTimeOfDay(this string time)
        {
            return TimeOfDay.Parse(time);
        }

        public static IEnumerable<string> GetProjectData(string filepath, DateTime now)
        {
            var database = LoadDatabase(filepath);
            var date = now.ToString("yyyy-MM-dd");
            var projects = database.Projects.Where(p => p.Date == date);

            var dict = new Dictionary<string, TimeSpan>();
            foreach (var p in projects)
            {
                var projectName = p.Name;
                if (!dict.ContainsKey(projectName)) dict.Add(projectName, new TimeSpan(0));
                dict[projectName] += p.GetTimeSpan();
            }

            foreach (var pair in dict) yield return $"{pair.Key}: {pair.Value.ToString(@"hh\:mm")}";
        }

        public static string GetCsv(string filepath)
        {
            var database = LoadDatabase(filepath);
            var textWriter = new StringWriter();
            var csv = new CsvWriter(textWriter, CultureInfo.CurrentCulture);

            var dates = new HashSet<string>();
            foreach (var project in database.Projects) dates.Add(project.Date);

            var datesList = dates.ToList();
            datesList.Sort();

            csv.WriteField("Data");
            foreach (var name in database.Names) csv.WriteField(name);
            csv.NextRecord();
            foreach (var date in datesList)
            {
                csv.WriteField(date);

                var projects = database.Projects.Where(p => p.Date == date);

                var dict = new Dictionary<string, TimeSpan>();
                foreach (var p in projects)
                {
                    var projectName = p.Name;
                    if (!dict.ContainsKey(projectName)) dict.Add(projectName, new TimeSpan(0));
                    dict[projectName] += p.GetTimeSpan();
                }

                foreach (var name in database.Names)
                    csv.WriteField(!dict.ContainsKey(name) ? null : dict[name].ToString(@"hh\:mm"));

                csv.NextRecord();
            }

            textWriter.Close();
            return textWriter.ToString();
        }

        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}