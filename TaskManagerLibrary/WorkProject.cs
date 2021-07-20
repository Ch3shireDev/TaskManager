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
            TimeBegin = DateTime.Now.ToString("HH:mm");
            TimeEnd = DateTime.Now.ToString("HH:mm");
            Name = projectName;
        }

        public string Name { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }

        public TimeSpan GetTimeSpan()
        {
            var date = DateTime.Parse(Date);
            var startDate = TimeBegin.AsTimeOfDay().ToDateTime(date);
            var endDate = TimeEnd.AsTimeOfDay().ToDateTime(date);
            return endDate - startDate;
        }
    }
}