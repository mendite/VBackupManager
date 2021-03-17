using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VBackupManager
{
    /// <summary>
    /// Interaction logic for BackupsHistoryView.xaml
    /// </summary>
    public partial class BackupsHistoryView : UserControl
    {
        BackupsHistoryViewModel viewModel;

        public BackupsHistoryView()
        {
            InitializeComponent();
            Loaded += BackupsHistoryView_Loaded;
        }

        private void BackupsHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel = DataContext as BackupsHistoryViewModel;
                if (viewModel == null)
                    return;

                viewModel.CommandReloadHistory.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
