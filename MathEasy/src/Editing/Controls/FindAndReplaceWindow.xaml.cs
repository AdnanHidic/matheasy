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
    /// Interaction logic for FindAndReplaceWindow.xaml
    /// </summary>
    public partial class FindAndReplaceWindow : Window
    {
        public FindAndReplaceWindow()
        {
            InitializeComponent();
            tb_find.Focus();
        }

        public Boolean MatchWholeWords { get { return (Boolean)cb_wholeWord.IsChecked; } }
        public String TextToFind { get { return tb_find.Text; } }
        public String TextToReplaceWith { get { return tb_replace.Text; } }
        public Boolean OperationOKd { get; private set; }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OperationOKd = false;
                this.Close();
            }
            else if (e.Key == Key.Enter && tb_replace.IsFocused)
            {
                OperationOKd = true;
                this.Close();
            }
        }

        private void b_replace_Click(object sender, RoutedEventArgs e)
        {
            OperationOKd = true;
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OperationOKd = false;
            this.Close();
        }
    }
}
