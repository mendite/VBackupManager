using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VBackupManager
{
    public class BackupCreationViewModel : NotifyPropertyChangedBase
    {
        #region Private fields        
        private object backupLockObject = new object();
        private bool isBackuping = false;
        #endregion

        #region Constructor
        public BackupCreationViewModel()
        {
            CommandClearComment = new RelayCommand(ExecuteCommandClearComment);
            CommandOpenValheimSaveGamesPath = new RelayCommand(ExcecuteCommandOpenValheimSaveGamesPath);
            CommandOpenArchivingPath = new RelayCommand(ExcecuteCommandOpenArchivingPath);
            CommandCreateBackup = new AsyncRelayCommand<object>(ExcecuteCommandCreateBackup, CanExcecuteCommandCreateBackup);
        }
        #endregion

        #region Commands
        public ICommand CommandClearComment { get; }
        public ICommand CommandOpenValheimSaveGamesPath { get; }
        public ICommand CommandOpenArchivingPath { get; }
        public IAsyncRelayCommand CommandCreateBackup { get; }

        #endregion

        #region Public properties
        private string comment = string.Empty;
        public string Comment
        {
            get { return comment; }
            set { base.OnPropertyChanged<string>(ref comment, value); }
        }

        private System.Windows.Media.Brush commentForeground = System.Windows.Media.Brushes.Black;
        public System.Windows.Media.Brush CommentForeground
        {
            get { return commentForeground; }
            set { base.OnPropertyChanged<System.Windows.Media.Brush>(ref commentForeground, value); }
        }

        private string messageText = string.Empty;
        public string MessageText
        {
            get { return messageText; }
            set { base.OnPropertyChanged<string>(ref messageText, value); }
        }
        #endregion

        #region Command methods        
        private void ExecuteCommandClearComment(object obj)
        {
            try
            {
                Comment = string.Empty;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExcecuteCommandOpenValheimSaveGamesPath(object obj)
        {
            try
            {
                Process.Start("explorer.exe", $"{VBackupSettingsManager.ValheimAppDataLocalLowSaveGamesPath}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExcecuteCommandOpenArchivingPath(object obj)
        {
            try
            {
                Process.Start("explorer.exe", $"{VBackupSettingsManager.LoadSettings().ArchivingPath}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private bool CanExcecuteCommandCreateBackup(object obj)
        {
            return !isBackuping;
        }

        private void ExcecuteCommandCreateBackup(CancellationToken cancellationToken, object obj)
        {
            try
            {
                try
                {
                    lock (backupLockObject)
                    {
                        if (isBackuping)
                            return;

                        isBackuping = true;
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        MessageText = string.Empty;
                        CommentForeground = System.Windows.Media.Brushes.Black;
                    });

                    var backupCreationReport = BackupManager.CreateBackup(VBackupSettingsManager.LoadSettings(), Comment);

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageText = backupCreationReport.Message;
                    });
                }
                finally
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Mouse.OverrideCursor = Cursors.Arrow;
                    });

                    lock (backupLockObject)
                        isBackuping = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageText = $"{ex?.Message}";
                    CommentForeground = System.Windows.Media.Brushes.Red;
                });
            }
        }
        #endregion
    }
}
