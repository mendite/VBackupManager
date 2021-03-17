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
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;

namespace VBackupManager
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            PreviewKeyDown += AboutWindow_PreviewKeyDown;
            Loaded += AboutWindow_Loaded;
        }

        private void AboutWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            TbAuthor.Text = FileVersionInfo.GetVersionInfo(typeof(AboutWindow).Assembly.Location).CompanyName;
            TbVersion.Text = FileVersionInfo.GetVersionInfo(typeof(AboutWindow).Assembly.Location).FileVersion;
            RtbLicence.AppendText(Properties.Resources.LicenceTxt);
        }

        private void AboutWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
