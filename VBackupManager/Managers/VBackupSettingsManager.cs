using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VBackupManager
{
    public static class VBackupSettingsManager
    {
        public static string ValheimAppDataLocalLowSaveGamesPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\LocalLow\IronGate\Valheim");
        public static string VBackupManagerLocalApplicationData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"VBackupManager");

        public static string VBackupSaveGameSettingsFile { get; } = Path.Combine(VBackupManagerLocalApplicationData, @"VBackupSaveGameSettings.json");

        public static void SaveSettings(BackupConfiguration saveGameBackupConfiguration)
        {
            if (saveGameBackupConfiguration == null)
                throw new ArgumentNullException(nameof(saveGameBackupConfiguration));

            if (!Directory.Exists(VBackupManagerLocalApplicationData))
                Directory.CreateDirectory(VBackupManagerLocalApplicationData);
            
            File.WriteAllText(VBackupSaveGameSettingsFile, JsonSerializer.Serialize<BackupConfiguration>(saveGameBackupConfiguration));
        }

        public static BackupConfiguration LoadSettings()
        {
            try
            {
                if (File.Exists(VBackupSaveGameSettingsFile))
                {
                    var configuration = JsonSerializer.Deserialize<BackupConfiguration>(File.ReadAllText(VBackupSaveGameSettingsFile));
                    if (configuration.BackupJobs == null)
                        configuration.BackupJobs = new ObservableCollection<BackupJobConfiguration>();

                    if (string.IsNullOrEmpty(configuration.ArchivingPath))
                        configuration.ArchivingPath = VBackupSettingsManager.VBackupManagerLocalApplicationData;

                    return configuration;
                }   

                return GetDefaultSaveGameBackupConfiguration();
            }
            catch
            {
                return GetDefaultSaveGameBackupConfiguration();
            }
        }

        public static BackupConfiguration GetDefaultSaveGameBackupConfiguration()
        {
            return new BackupConfiguration()
            {
                BackupJobs = new ObservableCollection<BackupJobConfiguration>(),
                ArchivingPath = VBackupSettingsManager.VBackupManagerLocalApplicationData,
        };
        }
    }
}
