using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace TaskManagerLibrary
{
    public class Database
    {
        public List<string> Names { get; set; } = new List<string>();
        public List<WorkProject> Projects { get; set; } = new List<WorkProject>();
        public string CurrentProject { get; set; }
        public string WorkDayStart { get; set; } = "08:00";
        public string WorkDayEnd { get; set; } = "16:00";
        public Connection Connection { get; set; }
        public List<string> DatesSent { get; set; }
        public void Update()
        {
            var project = Projects.LastOrDefault();
            if (project == null) return;
            var dateNow = Tools.GetCurrentDate();
            var timeNow = Tools.GetCurrentTime();
            if (!Names.Contains(CurrentProject)) CurrentProject = project.Name;
            if (project.Name != CurrentProject)
                project = new WorkProject
                {
                    Name = CurrentProject,
                    StartTime = project.EndTime,
                    EndTime = timeNow,
                    Date = dateNow
                };
            if (project.Date != dateNow)
            {
                project.EndTime = WorkDayEnd;

                project = new WorkProject
                {
                    Date = dateNow,
                    StartTime = WorkDayStart,
                    EndTime = timeNow,
                    Name = CurrentProject
                };

                Projects.Add(project);
            }
            else
            {
                project.EndTime = timeNow;
            }
        }

        public string GetCsv()
        {
            var textWriter = new StringWriter();
            var csv = new CsvWriter(textWriter, CultureInfo.CurrentCulture);

            var dates = new HashSet<string>();
            foreach (var project in Projects) dates.Add(project.Date);

            var datesList = dates.ToList();
            datesList.Sort();

            csv.WriteField("Data");
            foreach (var name in Names) csv.WriteField(name);
            csv.NextRecord();
            foreach (var date in datesList)
            {
                csv.WriteField(date);

                var projects = Projects.Where(p => p.Date == date);

                var dict = new Dictionary<string, TimeSpan>();
                foreach (var p in projects)
                {
                    var projectName = p.Name;
                    if (!dict.ContainsKey(projectName)) dict.Add(projectName, new TimeSpan(0));
                    dict[projectName] += p.GetTimeSpan();
                }

                foreach (var name in Names)
                    csv.WriteField(!dict.ContainsKey(name) ? null : dict[name].ToString(@"hh\:mm"));

                csv.NextRecord();
            }

            textWriter.Close();
            return textWriter.ToString();
        }

        public IEnumerable<string> GetProjectData()
        {
            var now = DateTime.Now;
            var date = now.ToString("yyyy-MM-dd");
            var projects = Projects.Where(p => p.Date == date);

            var dict = new Dictionary<string, TimeSpan>();
            foreach (var p in projects)
            {
                var projectName = p.Name;
                if (!dict.ContainsKey(projectName)) dict.Add(projectName, new TimeSpan(0));
                dict[projectName] += p.GetTimeSpan();
            }

            foreach (var pair in dict) yield return $"{pair.Key}: {pair.Value:hh\\:mm}";
        }

        public bool IsLastMonthSent()
        {
            var lastDateStr = DatesSent.LastOrDefault();
            if (lastDateStr == null) return false;
            var dateNow = DateTime.Now;
            var dateLast = DateTime.Parse(lastDateStr);
            if (dateLast.Month == dateNow.Month) return true;
            var x = dateLast.Month - 1;
            var y = dateNow.Month - 1;
            return (x + 1) % 12 == y;
        }
    }
}