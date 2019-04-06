using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using NaUKMA.CS.Practice05.Annotations;

namespace NaUKMA.CS.Practice05
{
    class TaskManagerVm : INotifyPropertyChanged
    {

        internal TaskManagerVm()
        {
            ProcessesUpdateSetup();
        }

        #region Commands

        private ICommand _stopProcessCommand;

        public ICommand StopProcessCommand
        {
            get
            {
                return _stopProcessCommand ?? (_stopProcessCommand = new RelayCommand<object>(
                           async o =>
                           {
                               await Task.Run(() => StopSelectedProcess());

                           }, o => ProcessSelectedCommand()));
            }
        }

        private bool ProcessSelectedCommand()
        {
            return !(SelectedProcess is null);
        }

        private ICommand _openFolderCommand;

        public ICommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand = new RelayCommand<object>(
                           async o =>
                           {
                               await Task.Run(() => OpenFolder());

                           }, o => CanOpenFolderCommand()));
            }
        }

        private bool CanOpenFolderCommand()
        {
            return !(SelectedProcess is null) && !(string.IsNullOrEmpty(SelectedProcess.FileName));
        }

        private ICommand _showModulesCommand;

        public ICommand ShowModulesCommand
        {
            get
            {
                return _showModulesCommand ?? (_showModulesCommand = new RelayCommand<object>(
                           async o =>
                           {
                               await Task.Run(() => ShowUsedModules());

                           }, o => ProcessSelectedCommand()));
            }
        }

        private ICommand _showThreadsCommand;

        public ICommand ShowThreadsCommand
        {
            get
            {
                return _showThreadsCommand ?? (_showThreadsCommand = new RelayCommand<object>(
                           async o =>
                           {
                               await Task.Run(() => ShowUsedThreads());

                           }, o => ProcessSelectedCommand()));
            }
        }

        #endregion

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

                if (!(SelectedProcess is null))
                {
                    bool selectedProcessStillAlive = false;
                    var currentModulesListTmp = CurrentModulesList;
                    var currentThreadsListTmp = CurrentThreadsList;
                    for (int i = 0; i < _processes.Count; i++)
                    {
                        MyProcess pr = _processes[i];
                        if (pr.ProcessId.Equals(SelectedProcess.ProcessId))
                        {
                            SelectedProcess = pr;
                            selectedProcessStillAlive = true;
                            break;
                        }
                    }

                    if (selectedProcessStillAlive.Equals(false))
                    {
                        SelectedProcess = null;
                        //CurrentModulesList = null;
                        //CurrentThreadsList = null;
                    }
                    else
                    { //show use modules/threads again after updating the processes list (if they were shown before the update was initiated)
                        if (!(currentModulesListTmp is null))
                        {
                            ShowUsedModules();
                        }

                        if (!(currentThreadsListTmp is null))
                        {
                            ShowUsedThreads();
                        }
                    }
                }

                OnPropertyChanged();
            }
        }
        private ObservableCollection<MyProcess> _processes;

        private async Task<ObservableCollection<MyProcess>> GetRefreshedProcesses()
        {
            ObservableCollection<MyProcess> newProcesses = new ObservableCollection<MyProcess>();
            Process[] currentProcessesArray = Process.GetProcesses();

            //call getmetadata (simult for all), then add all conseq.

            var countersCPU = new List<PerformanceCounter>();
            var countersRAM = new List<PerformanceCounter>();

            foreach (Process process in currentProcessesArray)
            {
                try
                {
                    var cpu = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
                    var ram = new PerformanceCounter("Process", "Private Bytes", process.ProcessName, true);

                    cpu.NextValue();
                    ram.NextValue();

                    countersCPU.Add(cpu);
                    countersRAM.Add(ram);
                }
                catch (Exception)
                {
                    //try-catch to suppress possible null pointer/arg exceptions due to stopped/changed process during the foreach process.
                    countersCPU.Add(null);
                    countersRAM.Add(null);
                    continue;
                }
            }

            Thread.Sleep(500);

            for (int i = 0; i < currentProcessesArray.Length; i++)
            {
                Process pr = currentProcessesArray[i];
                //null is a flag that some process was probably stopped, and there is no need to display it.
                if (countersCPU[i] is null || countersRAM[i] is null)
                {
                    continue;
                }
                Double cpuLoad = Math.Round(countersCPU[i].NextValue() / Environment.ProcessorCount, 2);
                Double ramUsed = Math.Round(countersRAM[i].NextValue() / 1024 / 1024, 2);
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

                string processOwner = GetProcessOwner(pr);

                newProcesses.Add(new MyProcess(pr.ProcessName, pr.Id, pr.Responding, pr.Threads.Count, startTime,
                    fileName, processOwner, cpuLoad, ramUsed));
            }

            return newProcesses;
        }

        private async void ProcessesUpdateSetup()
        {
            await Task.Run(() => UpdateProcessesList());
        }

        private async Task UpdateProcessesList()
        {
            while (true)
            {
                ObservableCollection<MyProcess> newProcesses = GetRefreshedProcesses().Result;
                await App.Current.Dispatcher.BeginInvoke((Action)delegate
               {
                   Processes = newProcesses;
               });
                Thread.Sleep(5000);
            }
        }

        #endregion

        #region MyProcessAdditional

        private static string GetProcessOwner(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out processHandle);
                WindowsIdentity wi = new WindowsIdentity(processHandle);
                string user = wi.Name;
                return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\") + 1) : user;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    CloseHandle(processHandle);
                }
            }
        }
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region SelectedProcessAndItsLogic

        public MyProcess SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                //if (_selectedProcess is null)
                //{
                //    _selectedProcess = value;
                //}
                //else if (!(_selectedProcess.Equals(value)))
                //{

                _selectedProcess = value;
                CurrentModulesList = null;
                CurrentThreadsList = null;
                //}
                OnPropertyChanged(nameof(SelectedProcess));
            }
        }

        private MyProcess _selectedProcess;

        public List<String> CurrentModulesList
        {
            get { return _currentModulesList; }
            set
            {
                _currentModulesList = value;
                OnPropertyChanged(nameof(CurrentModulesList));
            }
        }

        private List<String> _currentModulesList;

        private async Task StopSelectedProcess()
        {
            KillProcessAndChildren(SelectedProcess.ProcessId);
            await App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                Processes.Remove(SelectedProcess);
            });
            SelectedProcess = null;
        }

        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        private async Task OpenFolder()
        {
            string folderPath = SelectedProcess.FileName.Substring(0, SelectedProcess.FileName.LastIndexOf("\\"));
            Process.Start("explorer.exe", folderPath);
        }

        private async Task ShowUsedModules()
        {
            try
            {
                Process pr = Process.GetProcessById(SelectedProcess.ProcessId);
                List<String> mdlsList = new List<String>();
                foreach (ProcessModule processModule in pr.Modules)
                {
                    mdlsList.Add(processModule.ModuleName + ", " + processModule.FileName);
                }

                CurrentModulesList = mdlsList;
            }
            catch (Exception)
            {
                //x86 process can not access x64 modules
            }
        }

        public ProcessThreadCollection CurrentThreadsList
        {
            get { return _currentThreadsList; }
            set
            {
                _currentThreadsList = value;
                OnPropertyChanged(nameof(CurrentThreadsList));
            }
        }

        private ProcessThreadCollection _currentThreadsList;

        private async Task ShowUsedThreads()
        {
            Process pr = Process.GetProcessById(SelectedProcess.ProcessId);
            CurrentThreadsList = pr.Threads;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
