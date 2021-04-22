using System;

namespace TaskManagerLibrary
{
    public class Project
    {
        public Project(string name, int hours, int minutes)
        {
            Name = name;
            TimeSpan = new TimeSpan(hours, minutes, 0);
        }

        public string Name { get; set; }
        public TimeSpan TimeSpan { get; set; }

        public override string ToString()
        {
            return $"{Name}: {TimeSpan:hh\\:mm}";
        }
    }
}