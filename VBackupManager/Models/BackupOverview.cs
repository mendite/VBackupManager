using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VBackupManager
{
    public class BackupOverview : NotifyPropertyChangedBase
    {
        #region Private fields
        private string comment = string.Empty;
        private ObservableCollection<BackupJobInformation> items;
        private bool containsLocalSaveGames = false;
        private DateTime creationTime = DateTime.MinValue;
        #endregion

        #region Public properties
        [JsonPropertyName(nameof(Comment))]
        public string Comment
        {
            get { return comment; }
            set { base.OnPropertyChanged<string>(ref comment, value); }
        }

        [JsonPropertyName(nameof(BackupJobInformations))]
        public ObservableCollection<BackupJobInformation> BackupJobInformations
        {
            get { return items; }
            set { base.OnPropertyChanged<ObservableCollection<BackupJobInformation>>(ref items, value); }
        }

        [JsonPropertyName(nameof(ContainsLocalValheimSaveGames))]
        public bool ContainsLocalValheimSaveGames
        {
            get { return containsLocalSaveGames; }
            set { base.OnPropertyChanged<bool>(ref containsLocalSaveGames, value); }
        }

        [JsonPropertyName(nameof(CreationTime))]
        public DateTime CreationTime
        {
            get { return creationTime; }
            set { base.OnPropertyChanged<DateTime>(ref creationTime, value); }
        }
        #endregion
    }
}
