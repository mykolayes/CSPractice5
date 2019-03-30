﻿using System;
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
            set
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
            set
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
            set
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
            set
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
            set
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
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }
        [CanBeNull] private String _fileName;

        #endregion

        private MyProcess()
        {

        }

        public MyProcess(string processName, int processId, bool processResponding, int threadsCount, DateTime? startTime, string fileName)
        {
            ProcessName = processName;
            ProcessId = processId;
            ProcessResponding = processResponding;
            ThreadsCount = threadsCount;
            StartTime = startTime;
            FileName = fileName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
