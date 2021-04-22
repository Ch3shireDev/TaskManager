using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManagerLibrary;

namespace TaskManagerGUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string databasePath = "../../../database.yml";

        private int x = 0;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                database = Tools.LoadDatabase(databasePath);
            }
            catch(Exception e)
            {
                var message = MessageBox.Show($"Błąd - brak bazy danych! {e.Message}", "Błąd", MessageBoxButton.OKCancel);
                if (message == MessageBoxResult.Cancel) Close();
                var newDb = new TaskScheduleDatabase();
                Tools.SaveDatabase(newDb, databasePath);
                database = newDb;
            }

            foreach (var element in database.Names) ProjectComboBox.Items.Add(element);
            ProjectComboBox.SelectedItem = database.LastProjectName;

            CurrentProject = database.Projects.LastOrDefault();

            Tick();
        }

        public bool IsProjectRunning { get; set; }

        public TaskScheduleDatabase database { get; set; }

        public WorkProject CurrentProject { get; set; }

        public async Task Tick()
        {
            if (CurrentProject != null)
            {
                UpdateTime();
                Tools.SaveDatabase(database, databasePath);
            }

            await Task.Delay(1000);
            await Tick();
        }

        public void UpdateTime()
        {
            if (CurrentProject == null) return;

            var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            if (currentDate != CurrentProject?.Date && !string.IsNullOrWhiteSpace(CurrentProjectName))
            {
                CurrentProject= new WorkProject(CurrentProjectName);
            }

            CurrentProject.EndTime = TimeOfDay.Now.ToString();
            TimeSpentLabel.Content = (CurrentProject.EndTime.AsTimeOfDay() - CurrentProject.StartTime.AsTimeOfDay()).ToString();
        }

        private void OpenYamlButton_Click(object sender, RoutedEventArgs e)
        {
            var currentDir = Directory.GetCurrentDirectory();
            Process.Start(Path.Combine(currentDir, databasePath));
            Close();
        }

        private void StartWorkButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateProject();
        }

        private string CurrentProjectName => ProjectComboBox.SelectedItem as string;

        public void UpdateProject()
        {
            IsProjectRunning = !string.IsNullOrWhiteSpace(CurrentProjectName);
            if (string.IsNullOrWhiteSpace(CurrentProjectName)) return;
            var lastProject = database.Projects.LastOrDefault();
            if (lastProject == null || lastProject.Name != CurrentProjectName)
            {
                lastProject = new WorkProject(CurrentProjectName);
                database.Projects.Add(lastProject);
                database.LastProjectName = CurrentProjectName;
            }

            CurrentProject = lastProject;
            UpdateTime();
        }
        private void ProjectComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateProject();
        }
    }
}