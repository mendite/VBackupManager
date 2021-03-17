using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VBackupManager
{
    public interface IAsyncRelayCommand : ICommand
    {
        bool IsExecuting { get; }
    }
}
