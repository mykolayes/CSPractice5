using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NaUKMA.CS.Practice05.Annotations;

namespace NaUKMA.CS.Practice05
{
    class MyProcess : INotifyPropertyChanged
    {
        #region ProcessName

        public String ProcessName
        {
            get { return _processName; }
            private set
            {
                _processName = value;
                OnPropertyChanged("ProcessName");
            }
        }
        private String _processName;

        #endregion

        #region ProcessId

        public int ProcessId
        {
            get { return _processId; }
            private set
            {
                _processId = value;
                OnPropertyChanged("ProcessId");
            }
        }
        private int _processId;

        #endregion

        #region ProcessResponding

        public bool ProcessResponding
        {
            get { return _processResponding; }
            private set
            {
                _processResponding = value;
                OnPropertyChanged("ProcessResponding");
            }
        }
        private bool _processResponding;

        #endregion

        #region ThreadsCount

        public int ThreadsCount
        {
            get { return _threadsCount; }
            private set
            {
                _threadsCount = value;
                OnPropertyChanged("ThreadsCount");
            }
        }
        private int _threadsCount;

        #endregion

        #region StartTime

        public DateTime? StartTime
        {
            get { return _startTime; }
            private set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }
        private DateTime? _startTime;

        #endregion

        #region FileName

        [CanBeNull]
        public String FileName
        {
            get { return _fileName; }
            private set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }
        [CanBeNull] private String _fileName;

        #endregion

        #region ProcessOwner

        public String ProcessOwner
        {
            get { return _processOwner; }
            private set
            {
                _processOwner = value;
                OnPropertyChanged("ProcessOwner");
            }
        }
        private String _processOwner;

        #endregion

        #region CpuPercent

        public Double CpuPercent
        {
            get { return _cpuPercent; }
            private set
            {
                _cpuPercent = value;
                OnPropertyChanged("CpuPercent");
            }
        }
        private Double _cpuPercent;

        #endregion

        #region RamMB

        public Double RamMB
        {
            get { return _ramMB; }
            private set
            {
                _ramMB = value;
                OnPropertyChanged("RamMB");
            }
        }
        private Double _ramMB;

        #endregion

        private MyProcess()
        {

        }

        internal MyProcess(string processName, int processId, bool processResponding, int threadsCount, DateTime? startTime, string fileName, string processOwner, double cpuPercent, double ramMb)
        {
            ProcessName = processName;
            ProcessId = processId;
            ProcessResponding = processResponding;
            ThreadsCount = threadsCount;
            StartTime = startTime;
            FileName = fileName;
            ProcessOwner = processOwner;
            CpuPercent = cpuPercent;
            RamMB = ramMb;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
