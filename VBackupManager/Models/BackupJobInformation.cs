using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VBackupManager
{
    public class BackupJobInformation : NotifyPropertyChangedBase
    {
        #region Private fields
        private string name = string.Empty;
        private DateTime? lastModyfied;
        private string originalBackupPath = string.Empty;
        #endregion

        #region Public properties
        [JsonPropertyName(nameof(Name))]
        public string Name
        {
            get { return name; }
            set { base.OnPropertyChanged<string>(ref name, value); }
        }

        [JsonPropertyName(nameof(LastModyfied))]
        public DateTime? LastModyfied
        {
            get { return lastModyfied; }
            set { base.OnPropertyChanged<DateTime?>(ref lastModyfied, value); }
        }

        [JsonPropertyName(nameof(OriginalBackupPath))]
        public string OriginalBackupPath
        {
            get { return originalBackupPath; }
            set { base.OnPropertyChanged<string>(ref originalBackupPath, value); }
        }
        #endregion
    }
}
