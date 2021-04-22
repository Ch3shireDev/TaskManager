using System;

namespace TaskManagerLibrary
{
    public class WorkProject
    {
        public WorkProject()
        {
        }

        public WorkProject(string projectName)
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd");
            StartTime = DateTime.Now.ToString("HH:mm");
            EndTime = DateTime.Now.ToString("HH:mm");
            Name = projectName;
        }

        public string Name { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public TimeSpan GetTimeSpan()
        {
            var date = DateTime.Parse(Date);
            var startDate = StartTime.AsTimeOfDay().ToDateTime(date);
            var endDate = EndTime.AsTimeOfDay().ToDateTime(date);
            return endDate - startDate;
        }
    }
}