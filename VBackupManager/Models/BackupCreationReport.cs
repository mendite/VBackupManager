using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBackupManager
{
    public class BackupCreationReport : NotifyPropertyChangedBase
    {
        public BackupCreationReport(DateTime creationTime, string message)
        {
            CreationTime = creationTime;
            Message = message;
        }

        public DateTime CreationTime { get; }
        public string Message { get; }
    }
}
