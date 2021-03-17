using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VBackupManager
{
    public class BackupConfiguration : NotifyPropertyChangedBase
    {
        #region Private fields
        private ObservableCollection<BackupJobConfiguration> backupJobs;
        private bool backupLocalValheimSaveGames = true;
        private string archivingPath = string.Empty;
        #endregion

        #region Public properties
        [JsonPropertyName(nameof(BackupJobs))]
        public ObservableCollection<BackupJobConfiguration> BackupJobs
        {
            get { return backupJobs; }
            set { base.OnPropertyChanged<ObservableCollection<BackupJobConfiguration>>(ref backupJobs, value); }
        }

        [JsonPropertyName(nameof(BackupLocalValheimSaveGames))]
        public bool BackupLocalValheimSaveGames
        {
            get { return backupLocalValheimSaveGames; }
            set { base.OnPropertyChanged<bool>(ref backupLocalValheimSaveGames, value); }
        }

        [JsonPropertyName(nameof(ArchivingPath))]
        public string ArchivingPath
        {
            get { return archivingPath; }
            set { base.OnPropertyChanged<string>(ref archivingPath, value); }
        }
        #endregion
    }
}
