using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VBackupManager
{
    public class BackupSettingsViewModel : NotifyPropertyChangedBase
    {
        #region Constructor
        public BackupSettingsViewModel()
        {
            CommandLoadSettings = new RelayCommand(ExecuteCommandLoadSettings);
            CommandSaveSettings = new RelayCommand(ExecuteCommandSaveSettings);
            CommandCreateNewBackupJob = new RelayCommand(ExecuteCommandCreateNewBackupJob);
            CommandEditBackupJob = new RelayCommand<BackupJobConfiguration>(ExecuteCommandEditBackupJob);
            CommandCancelEditingBackupJob = new RelayCommand(ExecuteCommandCancelEditingBackupJob);
            CommandSaveEditingBackupJob = new RelayCommand(ExecuteCommandSaveEditingBackupJob);
            CommandDeleteBackupJob = new RelayCommand<BackupJobConfiguration>(ExecuteCommandDeleteBackupJob);
            CommandSelectPathForCreateNewBackupJob = new RelayCommand(ExecuteCommandSelectPathForCreateNewBackupJob);
            CommandSelectSavePathForBackupArchiving = new RelayCommand(ExecuteCommandSelectSavePathForBackupArchiving);
            CommandSetDefaultSavePathForBackupArchiving = new RelayCommand(ExecuteCommandSetDefaultSavePathForBackupArchiving);
        }
        #endregion

        #region Commands
        public ICommand CommandLoadSettings { get; }
        public ICommand CommandSaveSettings { get; }
        public ICommand CommandCreateNewBackupJob { get; }
        public ICommand CommandEditBackupJob { get; }
        public ICommand CommandCancelEditingBackupJob { get; }
        public ICommand CommandSaveEditingBackupJob { get; }
        public ICommand CommandDeleteBackupJob { get; }
        public ICommand CommandSelectPathForCreateNewBackupJob { get; }
        public ICommand CommandSelectSavePathForBackupArchiving { get; }
        public ICommand CommandSetDefaultSavePathForBackupArchiving { get; }
        #endregion

        #region Public properties
        private BackupJobConfiguration stateOfOldBackupJobConfiguration;
        public BackupJobConfiguration StateOfOldBackupJobConfiguration
        {
            get { return stateOfOldBackupJobConfiguration; }
            set { base.OnPropertyChanged<BackupJobConfiguration>(ref stateOfOldBackupJobConfiguration, value); }
        }

        private BackupJobConfiguration currentEditingBackupJobConfiguration;
        public BackupJobConfiguration CurrentEditingBackupJobConfiguration
        {
            get { return currentEditingBackupJobConfiguration; }
            set { base.OnPropertyChanged<BackupJobConfiguration>(ref currentEditingBackupJobConfiguration, value); }
        }

        private BackupConfiguration configuration = VBackupSettingsManager.LoadSettings();
        public BackupConfiguration Configuration
        {
            get { return configuration; }
            set 
            {
                StateOfOldBackupJobConfiguration = null;
                CurrentEditingBackupJobConfiguration = null;
                base.OnPropertyChanged<BackupConfiguration>(ref configuration, value);
            }
        }
        #endregion

        #region Command methods     
        private void ExecuteCommandLoadSettings(object obj)
        {
            try
            {
                Configuration = VBackupSettingsManager.LoadSettings();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandSaveSettings(object obj)
        {
            try
            {
                StateOfOldBackupJobConfiguration = null;
                CurrentEditingBackupJobConfiguration = null;
                VBackupSettingsManager.SaveSettings(Configuration);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandCreateNewBackupJob(object obj)
        {
            try
            {
                StateOfOldBackupJobConfiguration = null;
                CurrentEditingBackupJobConfiguration = new BackupJobConfiguration();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandEditBackupJob(BackupJobConfiguration item)
        {
            try
            {
                StateOfOldBackupJobConfiguration = item;
                CurrentEditingBackupJobConfiguration = item.Clone() as BackupJobConfiguration;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandCancelEditingBackupJob(object obj)
        {
            try
            {
                StateOfOldBackupJobConfiguration = null;
                CurrentEditingBackupJobConfiguration = null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandSaveEditingBackupJob(object obj)
        {
            try
            {
                if (CurrentEditingBackupJobConfiguration != null)
                {
                    if (string.IsNullOrEmpty(CurrentEditingBackupJobConfiguration.Name) ||
                        ($"{StateOfOldBackupJobConfiguration?.Name}" == $"{CurrentEditingBackupJobConfiguration?.Name}" && Configuration.BackupJobs.Where(X => X.Name.ToLower() == CurrentEditingBackupJobConfiguration.Name).Count() > 1) ||
                        ($"{StateOfOldBackupJobConfiguration?.Name}" != $"{CurrentEditingBackupJobConfiguration?.Name}" && Configuration.BackupJobs.Where(X => X.Name.ToLower() == CurrentEditingBackupJobConfiguration.Name).Count() > 0))
                        throw new InvalidOperationException(Properties.Resources.Only_unique_non_empty_names_are_allowed_for_backup_jobs_);

                    if (StateOfOldBackupJobConfiguration == null)
                    {
                        Configuration.BackupJobs.Add(CurrentEditingBackupJobConfiguration);
                    }
                    else
                    {
                        StateOfOldBackupJobConfiguration.Name = CurrentEditingBackupJobConfiguration.Name;
                        StateOfOldBackupJobConfiguration.Path = CurrentEditingBackupJobConfiguration.Path;
                        StateOfOldBackupJobConfiguration.BackupingIsActive = CurrentEditingBackupJobConfiguration.BackupingIsActive;
                    }
                }

                CurrentEditingBackupJobConfiguration = null;
                StateOfOldBackupJobConfiguration = null;

                CommandSaveSettings.Execute(null);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandDeleteBackupJob(BackupJobConfiguration backupJob)
        {
            try
            {
                if (backupJob == null)
                    return;

                var dialogResult = System.Windows.MessageBox.Show(Properties.Resources.Delete_selected_backup_job_, Properties.Resources.Delete, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, System.Windows.MessageBoxResult.No);

                if (dialogResult != System.Windows.MessageBoxResult.Yes)
                    return;

                Configuration.BackupJobs.Remove(backupJob);
                CommandSaveSettings.Execute(null);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandSelectPathForCreateNewBackupJob(object obj)
        {
            try
            {
                if (CurrentEditingBackupJobConfiguration == null)
                    return;

                var dialog = new System.Windows.Forms.FolderBrowserDialog()
                {                    
                    Description = Properties.Resources.Path_to_backup,
                    SelectedPath = CurrentEditingBackupJobConfiguration.Path,
                    ShowNewFolderButton = true,
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    CurrentEditingBackupJobConfiguration.Path = dialog.SelectedPath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandSelectSavePathForBackupArchiving(object obj)
        {
            try
            {                
                var dialog = new System.Windows.Forms.FolderBrowserDialog()
                {
                    Description = Properties.Resources.Select_archiving_path,
                    SelectedPath = Configuration.ArchivingPath,
                    ShowNewFolderButton = true,
                };

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    Configuration.ArchivingPath = dialog.SelectedPath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, Properties.Resources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteCommandSetDefaultSavePathForBackupArchiving(object obj)
        {
            Configuration.ArchivingPath = VBackupSettingsManager.VBackupManagerLocalApplicationData;
        }
        #endregion
    }
}
