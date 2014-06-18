using System;

namespace FutureState.AppCore.Data.Models
{
    public class DatabaseVersionModel
    {
        public int VersionNumber { get; set; }
        public bool IsBeforeMigrationComplete { get; set; }
        public bool IsMigrationComplete { get; set; }
        public bool IsAfterMigrationComplete { get; set; }
        public DateTime MigrationDate { get; set; }
    }
}