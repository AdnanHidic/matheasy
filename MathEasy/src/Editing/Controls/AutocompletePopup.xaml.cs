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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace MathEasy.src.Editing.Controls
{
    /// <summary>
    /// Interaction logic for AutocompletePopup.xaml
    /// </summary>
    public partial class AutocompletePopup : Popup
    {
        public event EventHandler returnsFocus = delegate { };

        public AutocompletePopup()
        {
            InitializeComponent();
            scrollviewer.PreviewMouseWheel += new MouseWheelEventHandler(scrollviewer_PreviewMouseWheel);
        }

        void scrollviewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void lb_actions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            returnsFocus(null, null);
        }
    }
}
