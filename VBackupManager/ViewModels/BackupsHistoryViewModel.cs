using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VBackupManager
{
    public class BackupsHistoryViewModel : NotifyPropertyChangedBase
    {
        #region Private fields
        private object reloadLockObject = new object();
        #endregion

        #region Constructor
        public BackupsHistoryViewModel()
        {
            CommandOpenBackupHistoryItem = new RelayCommand<BackupHistoryItem>(ExecuteCommandOpenBackupHistoryItem);
            CommandReloadHistory = new RelayCommand(ExecuteCommandReloadHistory);
        }
        #endregion

        #region Commands
        public ICommand CommandOpenBackupHistoryItem { get; }
        public ICommand CommandReloadHistory { get; }
        #endregion

        #region Public properties
        public ObservableCollection<BackupHistoryItem> BackupHistory { get; } = new ObservableCollection<BackupHistoryItem>();
        #endregion

        #region Command methods
        private void ExecuteCommandOpenBackupHistoryItem(BackupHistoryItem backupHistoryItem)
        {
            try
            {
                if (backupHistoryItem == null)
                    return;

                if (!System.IO.File.Exists(backupHistoryItem.Filename))
                    throw new System.IO.FileNotFoundException(string.Format(Properties.Resources.Backup_file___0___not_found_, backupHistoryItem.Filename), backupHistoryItem.Filename);
                
                Process.Start("explorer.exe", $"{backupHistoryItem.Filename}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandReloadHistory(object obj)
        {
            try
            {
                ReloadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Private methods
        private void ReloadHistory()
        {
            lock (reloadLockObject)
            {
                try
                {
                    List<BackupHistoryItem> itemsInHistory = new List<BackupHistoryItem>();
                    List<BackupHistoryItem> itemsToAdd = new List<BackupHistoryItem>();
                    List<BackupHistoryItem> itemsToRemove = new List<BackupHistoryItem>();

                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });

                    Application.Current.Dispatcher.Invoke(() => { itemsInHistory = BackupHistory.ToList(); });

                    List<BackupHistoryItem> itemsInDirectory = BackupManager.GetBackupHistoryItems(VBackupSettingsManager.LoadSettings().ArchivingPath).ToList();

                    foreach (var backupHistoryItem in itemsInDirectory)
                        if (itemsInHistory.Where(X => X.Filename == backupHistoryItem.Filename).Count() <= 0)
                            itemsToAdd.Add(backupHistoryItem);

                    foreach (var backupHistoryItem in itemsInHistory)
                        if (itemsInDirectory.Where(X => X.Filename == backupHistoryItem.Filename).Count() <= 0)
                            itemsToRemove.Add(backupHistoryItem);

                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        foreach (var backupItem in itemsToRemove)
                            BackupHistory.Remove(backupItem);

                        foreach (var backupItem in itemsToAdd)
                            BackupHistory.Add(backupItem);

                    });
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Arrow; });
                }
            }
        }
        #endregion
    }
}
