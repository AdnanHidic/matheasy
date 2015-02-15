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

namespace MathEasy.src.Rendering
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public event ReloadRequestedHandler ReloadRequested;

        public PreviewWindow()
        {
            InitializeComponent();
        }

        public PreviewWindow(String htmlToPreview)
        {
            InitializeComponent();
            pbox.loadHTML(htmlToPreview);
        }

        public void loadHTMLToPreview(String htmlToPreview)
        {
            pbox.loadHTML(htmlToPreview);
        }

        private void RefreshContent(object o, RoutedEventArgs e)
        {
            if (ReloadRequested != null)
                ReloadRequested(this);
        }
    }

    public delegate void ReloadRequestedHandler(object sender);
}
