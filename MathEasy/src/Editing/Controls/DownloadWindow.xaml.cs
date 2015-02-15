using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MathEasy.src.Editing.Controls
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        public DownloadWindow()
        {
            InitializeComponent();
            tb_url.Focus();
        }

        private String extractAccessCodeFromURL(String URL)
        {
            int lastSlashIndex = URL.LastIndexOf('/');
            if (lastSlashIndex >= URL.Length+1 || lastSlashIndex == -1)
                return "";
            return URL.Substring(lastSlashIndex + 1);
        }

        public String ACCESSCODE { get { return extractAccessCodeFromURL(tb_url.Text); } }
        public Boolean ShouldExecute { get; private set; }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ShouldExecute = false;
                this.Close();
            }
        }

        private void b_dl_Click(object sender, RoutedEventArgs e)
        {
            ShouldExecute = true;
            this.Close();
        }

        private void b_cancel_Click(object sender, RoutedEventArgs e)
        {
            ShouldExecute = false;
            this.Close();
        }
    }
}
