using System.Collections.Generic;
using System.Linq;

namespace TaskManagerLibrary
{
    public class TaskScheduleDatabase
    {
        public List<string> Names { get; set; } = new List<string>();
        public List<WorkProject> Projects { get; set; } = new List<WorkProject>();
        public string CurrentProject { get; set; }
        public string WorkDayStart { get; set; } = "08:00";
        public string WorkDayEnd { get; set; } = "16:00";
        public Connection Connection { get; set; }

        public void UpdateCurrentProject()
        {
            var project = Projects.LastOrDefault();
            if (project == null) return;
            var now = Tools.GetCurrentDate();
            CurrentProject = project.Name;
            if (project.Date != now)
                project = new WorkProject
                {
                    Date = now,
                    StartTime = WorkDayStart
                };
        }
    }

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