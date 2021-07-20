using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskManagerLibrary;

namespace TaskManagerGUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string databasePath = "../../../database.yml";

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                database = Tools.LoadDatabase(databasePath);
            }
            catch
            {
                var message = MessageBox.Show("Błąd - brak bazy danych! Czy utworzyć nową bazę danych?", "Błąd",
                    MessageBoxButton.OKCancel);
                if (message == MessageBoxResult.Cancel) Close();
                if (File.Exists(databasePath)) File.Copy(databasePath, databasePath + "-copy");
                var newDb = new Database();
                Tools.SaveDatabase(newDb, databasePath);
                database = newDb;
                Environment.Exit(0);
            }

            foreach (var element in database.Names) ProjectComboBox.Items.Add(element);
            ProjectComboBox.SelectedItem = database.CurrentProject;

            CurrentProject = database.Projects.LastOrDefault();

            //Tick();
        }

        public bool IsProjectRunning { get; set; }

        public Database database { get; set; }

        public WorkProject CurrentProject { get; set; }

        private string CurrentProjectName => ProjectComboBox.SelectedItem as string;



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

        public void UpdateProject()
        {
            IsProjectRunning = !string.IsNullOrWhiteSpace(CurrentProjectName);
            if (string.IsNullOrWhiteSpace(CurrentProjectName)) return;
            database.CurrentProject = CurrentProjectName;
            database.Update();
            Tools.SaveDatabase(database, databasePath);
        }

        private void ProjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProject();
        }
    }
}