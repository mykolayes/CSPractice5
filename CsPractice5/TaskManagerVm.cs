using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NaUKMA.CS.Practice05.Annotations;

namespace NaUKMA.CS.Practice05
{
    class TaskManagerVm : INotifyPropertyChanged
    {

        internal TaskManagerVm()
        {
            ProcessesUpdateSetup();
        }

        #region Processes

        public ObservableCollection<MyProcess> Processes
        {
            get
            {
                return _processes;
            }
            set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<MyProcess> _processes;

        private ObservableCollection<MyProcess> GetRefreshedProcesses()
        {
            ObservableCollection<MyProcess> newProcesses = new ObservableCollection<MyProcess>();
            Process[] currentProcessesArray = Process.GetProcesses();
            foreach (var pr in currentProcessesArray)
            {
                DateTime? startTime;
                string fileName;
                try
                {
                    startTime = pr.StartTime;
                }
                catch (Exception)
                {
                    startTime = null;
                }

                try
                {
                    fileName = pr.MainModule.FileName;
                }
                catch (Exception)
                {
                    fileName = null;
                }
                newProcesses.Add(new MyProcess(pr.ProcessName, pr.Id, pr.Responding, pr.Threads.Count, startTime, fileName));
            }

            return newProcesses;
        }

        #endregion

        private async void ProcessesUpdateSetup()
        {
            await Task.Run(() => UpdateProcessesList());
        }

        private async Task UpdateProcessesList()
        {
            while (true)
            {
                ObservableCollection<MyProcess> newProcesses = GetRefreshedProcesses();
                await App.Current.Dispatcher.BeginInvoke((Action) delegate
                {
                    Processes = newProcesses;
                });
                Thread.Sleep(5000);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
