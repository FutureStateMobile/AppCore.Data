using System;
using Mono.Data.Sqlite;

namespace FutureState.AppCore.Data.Sqlite
{
    public class SqliteSettings
    {
        // Default Values
        //-----------------------------
        private int _cacheSize = 2000;
        private TimeSpan _defaultTimeout = TimeSpan.FromSeconds(0.1);
        private bool _failIfMissing = true;
        private SQLiteJournalModeEnum _journalMode = SQLiteJournalModeEnum.Delete;
        private int _pageSize = 1024;
        private SynchronizationModes _syncMode = SynchronizationModes.Normal;
        //-----------------------------

        public SQLiteJournalModeEnum JournalMode
        {
            get { return _journalMode; }
            set { _journalMode = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value > 65536)
                {
                    _cacheSize = 65536;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }

        public TimeSpan DefaultTimeout
        {
            get { return _defaultTimeout; }
            set { _defaultTimeout = value; }
        }

        public SynchronizationModes SyncMode
        {
            get { return _syncMode; }
            set { _syncMode = value; }
        }

        public int CacheSize
        {
            get { return _cacheSize; }
            set { _cacheSize = value; }
        }

        public bool FailIfMissing
        {
            get { return _failIfMissing; }
            set { _failIfMissing = value; }
        }

        public bool ReadOnly { get; set; }
    }
}