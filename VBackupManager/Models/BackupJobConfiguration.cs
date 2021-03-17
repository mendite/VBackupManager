using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VBackupManager
{
    public class BackupJobConfiguration : NotifyPropertyChangedBase, ICloneable
    {
        #region Private fields
        private string name = string.Empty;
        private string path = string.Empty;
        private bool backupingIsActive = true;
        #endregion

        #region Public properties
        [JsonPropertyName(nameof(Name))]
        public string Name
        {
            get { return name; }
            set { base.OnPropertyChanged<string>(ref name, value); }
        }

        [JsonPropertyName(nameof(Path))]
        public string Path
        {
            get { return path; }
            set { base.OnPropertyChanged<string>(ref path, value); }
        }

        [JsonPropertyName(nameof(BackupingIsActive))]
        public bool BackupingIsActive
        {
            get { return backupingIsActive; }
            set { base.OnPropertyChanged<bool>(ref backupingIsActive, value); }
        }

        public object Clone()
        {
            return new BackupJobConfiguration()
            {
                Name = Name,
                Path = Path,
                BackupingIsActive = BackupingIsActive,
            };
        }
        #endregion
    }
}
