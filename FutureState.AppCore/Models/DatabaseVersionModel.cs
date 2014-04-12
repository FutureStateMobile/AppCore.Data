using System;

namespace FutureState.AppCore.Data.Models
{
    public class DatabaseVersionModel
    {
        public int VersionNumber { get; set; }
        public DateTime MigrationDate { get; set; }
    }
}