using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VBackupManager
{
    public class BackupHistoryItem : NotifyPropertyChangedBase
    {
        #region Private fields
        #endregion

        #region Constructor
        public BackupHistoryItem(string filename, BackupOverview overview)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            if (overview == null)
                throw new ArgumentNullException(nameof(overview));

            Filename = filename;
            Overview = overview;
        }
        #endregion

        #region Public properties
        public string Filename { get; }
        public BackupOverview Overview { get; }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        #endregion
    }
}
