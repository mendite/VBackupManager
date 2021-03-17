using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VBackupManager
{
    public static class BackupManager
    {
        #region Public fields
        public const string BackupOverviewFilename = "BackupOverview.json";
        public const string LocalValheimSaveGamesName = "LocalValheimSaveGames";
        public const string BackupFileDateTimeFormat = "yyyyMMddHHmmss";
        public const string BackupFilenameHead = "Backup_";
        #endregion

        #region Public methods
        public static BackupCreationReport CreateBackup(BackupConfiguration saveGameBackupConfiguration, string comment)
        {
            if (saveGameBackupConfiguration == null)
                throw new ArgumentNullException(nameof(saveGameBackupConfiguration));

            if (!saveGameBackupConfiguration.BackupLocalValheimSaveGames && saveGameBackupConfiguration.BackupJobs.Where(X=>X.BackupingIsActive).Count() <= 0)
                throw new InvalidOperationException(Properties.Resources.No_backup_can_be_created_because_no_data_to_be_backed_up_is_defined);

            CreateArchivingPathIfNotExists(saveGameBackupConfiguration);

            var backupCreationTime = DateTime.Now;
            string zipArchivFilename = CreateZipArchivFilename(backupCreationTime, saveGameBackupConfiguration);

            CreateZipArchive(saveGameBackupConfiguration, zipArchivFilename, comment);

            return new BackupCreationReport(backupCreationTime, string.Format(Properties.Resources.Backup_was_created_at__0_, backupCreationTime));
        }

        public static void CreateArchivingPathIfNotExists(BackupConfiguration saveGameBackupConfiguration)
        {
            if (saveGameBackupConfiguration == null)
                throw new ArgumentNullException(nameof(saveGameBackupConfiguration));

            if (!Directory.Exists(saveGameBackupConfiguration.ArchivingPath))
                Directory.CreateDirectory(saveGameBackupConfiguration.ArchivingPath);
        }

        public static string CreateZipArchivFilename(DateTime backupCreationTime, BackupConfiguration saveGameBackupConfiguration)
        {
            if (saveGameBackupConfiguration == null)
                throw new ArgumentNullException(nameof(saveGameBackupConfiguration));

            return Path.Combine(saveGameBackupConfiguration.ArchivingPath, $"{BackupFilenameHead}{backupCreationTime.ToString(BackupFileDateTimeFormat)}.zip");
        }

        public static bool FilenameIsMatchBackupItem(string filename)
        {
            return Regex.IsMatch(Path.GetFileName(filename).ToLower(), $"{BackupFilenameHead}\\d{{14}}.zip".ToLower());
        }

        public static DateTime GetDateTimeFromBackupFile(string filename)
        {
            return DateTime.ParseExact(Path.GetFileNameWithoutExtension(filename).Replace(BackupFilenameHead, string.Empty), BackupFileDateTimeFormat, null);
        }

        public static void CreateZipArchive(BackupConfiguration saveGameBackupConfiguration, string zipArchivFilename, string comment)
        {
            if (saveGameBackupConfiguration == null)
                throw new ArgumentNullException(nameof(saveGameBackupConfiguration));

            if (zipArchivFilename == null)
                throw new ArgumentNullException(nameof(zipArchivFilename));

            if (Path.GetExtension(zipArchivFilename).ToLower() != ".zip")
                throw new ArgumentOutOfRangeException(nameof(zipArchivFilename), $"Value of argument {nameof(zipArchivFilename)} ending not with extension .zip.");

            if (saveGameBackupConfiguration.BackupLocalValheimSaveGames)
            {
                if (!Directory.Exists(VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath))
                    throw new DirectoryNotFoundException(string.Format(Properties.Resources.Can_t_create_backup__because_directory_for_save_games_is_missed__0__, VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath));

                if (Directory.GetFiles(VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath, "*", SearchOption.AllDirectories).Length <= 0)
                    throw new InvalidOperationException(string.Format(Properties.Resources.Can_t_create_backup__because_no_file_was_found_in_directory___0___, VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath));
            }

            foreach (var backupJobConfiguration in saveGameBackupConfiguration.BackupJobs)
                if (backupJobConfiguration.BackupingIsActive && !Directory.Exists(backupJobConfiguration.Path))
                    throw new DirectoryNotFoundException(string.Format(Properties.Resources.Can_t_create_backup__because_path___0___of_job___1___can_t_be_found_, new object[] { backupJobConfiguration.Path , backupJobConfiguration.Name }));

            using (FileStream fileStream = new FileStream(zipArchivFilename, FileMode.CreateNew, FileAccess.Write))
            {
                using (ZipArchive zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create))
                {
                    BackupOverview backupOverview = new BackupOverview()
                    {
                        Comment = $"{comment}",
                        BackupJobInformations = new ObservableCollection<BackupJobInformation>(),
                        CreationTime = DateTime.UtcNow,
                    };

                    if (saveGameBackupConfiguration.BackupLocalValheimSaveGames)
                    {
                        backupOverview.ContainsLocalValheimSaveGames = true;

                        var saveGameBackupInformation = new BackupJobInformation();
                        saveGameBackupInformation.Name = LocalValheimSaveGamesName;
                        saveGameBackupInformation.OriginalBackupPath = VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath;
                        backupOverview.BackupJobInformations.Add(saveGameBackupInformation);

                        foreach (var filename in Directory.GetFiles(VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath, "*", SearchOption.AllDirectories))
                        {
                            DateTime lastWriteTime = new FileInfo(filename).LastWriteTime;
                            if (!saveGameBackupInformation.LastModyfied.HasValue || saveGameBackupInformation.LastModyfied.Value < lastWriteTime)
                                saveGameBackupInformation.LastModyfied = lastWriteTime;

                            string zipArchiveEntryName = filename.Substring(VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath.Length);
                            if (zipArchiveEntryName.StartsWith("\\"))
                                zipArchiveEntryName = zipArchiveEntryName.Substring(1);

                            var zipArchiveEntry = zipArchive.CreateEntryFromFile(filename, $"{saveGameBackupInformation.Name}\\{zipArchiveEntryName}", CompressionLevel.Optimal);
                        }
                    }

                    foreach (var backupJobConfiguration in saveGameBackupConfiguration.BackupJobs)
                    {
                        if (!backupJobConfiguration.BackupingIsActive)
                            continue;

                        var saveGameBackupInfoItem = new BackupJobInformation();
                        saveGameBackupInfoItem.Name = backupJobConfiguration.Name;
                        saveGameBackupInfoItem.OriginalBackupPath = backupJobConfiguration.Path;
                        backupOverview.BackupJobInformations.Add(saveGameBackupInfoItem);

                        foreach (var filename in Directory.GetFiles(backupJobConfiguration.Path))
                        {
                            DateTime lastWriteTime = new FileInfo(filename).LastWriteTime;
                            if (!saveGameBackupInfoItem.LastModyfied.HasValue || saveGameBackupInfoItem.LastModyfied.Value < lastWriteTime)
                                saveGameBackupInfoItem.LastModyfied = lastWriteTime;

                            string zipArchiveEntryName = filename.Substring(backupJobConfiguration.Path.Length);
                            if (zipArchiveEntryName.StartsWith("\\"))
                                zipArchiveEntryName = zipArchiveEntryName.Substring(1);

                            var zipArchiveEntry = zipArchive.CreateEntryFromFile(filename, $"{saveGameBackupInfoItem.Name}\\{zipArchiveEntryName}", CompressionLevel.Optimal);
                        }
                    }

                    ZipArchiveEntry zipArchiveEntrySaveGameBackupInfo = zipArchive.CreateEntry(BackupOverviewFilename);
                    using (StreamWriter streamWriter = new StreamWriter(zipArchiveEntrySaveGameBackupInfo.Open()))
                        streamWriter.Write(JsonSerializer.Serialize<BackupOverview>(backupOverview));
                }
            }
        }

        public static BackupOverview GetBackupInformationItemByZippedFile(string zipArchivFilename)
        {
            if (zipArchivFilename == null)
                throw new ArgumentNullException(nameof(zipArchivFilename));

            if (Path.GetExtension(zipArchivFilename).ToLower() != ".zip")
                throw new ArgumentOutOfRangeException(nameof(zipArchivFilename), $"Value of argument {nameof(zipArchivFilename)} ending not with extension .zip.");

            using (ZipArchive zipArchive = ZipFile.OpenRead(zipArchivFilename))
            {
                if (zipArchive.Entries.Where(X => X.Name.ToLower() == BackupOverviewFilename.ToLower()).Count() <= 0)
                {
                    return new BackupOverview()
                    {
                        Comment = $"Backuped {GetDateTimeFromBackupFile(zipArchivFilename)}",
                        ContainsLocalValheimSaveGames = false,
                        BackupJobInformations = new ObservableCollection<BackupJobInformation>(),
                    };
                }
                else
                {
                    ZipArchiveEntry saveGamesBackupInfoEntry = zipArchive.GetEntry(BackupOverviewFilename);
                    using (Stream saveGameBackupInfoStream = saveGamesBackupInfoEntry.Open())
                    {
                        using (MemoryStream saveGameBackupInfoMemoryStream = new MemoryStream())
                        {
                            saveGameBackupInfoStream.CopyTo(saveGameBackupInfoMemoryStream);
                            var backupOverview = (BackupOverview)JsonSerializer.Deserialize<BackupOverview>(saveGameBackupInfoMemoryStream.ToArray());
                            return backupOverview;
                        }
                    }
                }
            }
        }

        public static IEnumerable<BackupHistoryItem> GetBackupHistoryItems(string archivingPath)
        {
            if (!Directory.Exists(archivingPath))
                yield break;

            foreach (var filename in Directory.GetFiles(archivingPath, $"{BackupFilenameHead}*.zip", SearchOption.TopDirectoryOnly))
                yield return new BackupHistoryItem(filename, GetBackupInformationItemByZippedFile(filename));
        }
        #endregion
    }
}
