using System.Windows;

namespace NaUKMA.CS.Practice05
{
    /// <summary>
    /// Interaction logic for TaskManagerWindow.xaml
    /// </summary>
    public partial class TaskManagerWindow : Window
    {
        public TaskManagerWindow()
        {
            InitializeComponent();
            DataContext = new TaskManagerVm();
        }
    }
}
