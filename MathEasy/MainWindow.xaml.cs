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

using MathEasy.src.Editing.Controls;
using MathEasy.src.Rendering;
using MathEasy.src.LMD.Core;
using MathEasy.src.ConfigurationManagement;
using MathEasy.src.FileManagement;

namespace MathEasy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PreviewWindow pwindow;

        public MainWindow()
        {
            System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = (System.IntPtr)1;
            InitializeComponent();

            PreviewIsOpen = false;
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            setWindowTitle("untitled");
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            MathEasyCommands.InitializeCommands();
            BindCommands();
        }

        private void BindCommands()
        {
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.NewDocument, NewDocumentExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.Save, SaveDocumentExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.Upload, UploadDocumentExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.Open, OpenDocumentExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.Download, DownloadDocumentExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.Preview, PreviewExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.ExitProgram, ExitProgramExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.Undo, UndoExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.Redo, RedoExec));
            this.CommandBindings.Add(new CommandBinding(MathEasyCommands.FindAndReplace, FindReplaceExec));
        }

        private void NewDocumentExec(object sender, ExecutedRoutedEventArgs e)
        {
            setWindowTitle(DocumentManager.newDocumentProcedure(mbox.Document));
        }

        private void setWindowTitle(String s)
        {
            if (s == null || s == "")
                return;
            String newTitle = "[" + s + "] - MathEasy RC";
            this.Title = newTitle;
        }

        private void SaveDocumentExec(object sender, ExecutedRoutedEventArgs e)
        {
            setWindowTitle(DocumentManager.saveProcedure(mbox.Document));
        }

        private void UploadDocumentExec(object sender, ExecutedRoutedEventArgs e)
        {
            String s = DocumentManager.uploadProcedure(mbox.Document);
            if (s != "")
            {
                s = ConfigurationManager.URL + "/get/" + s;
                UploadWindow uw = new UploadWindow(s);
                uw.ShowDialog();
            }
        }

        private void OpenDocumentExec(object sender, ExecutedRoutedEventArgs e)
        {
            setWindowTitle(DocumentManager.loadProcedure(mbox.Document));
        }

        private void DownloadDocumentExec(object sender, ExecutedRoutedEventArgs e)
        {
            DownloadWindow dw = new DownloadWindow();
            dw.ShowDialog();
            if (dw.ShouldExecute && dw.ACCESSCODE != "")
            {
                DocumentManager.downloadProcedure(dw.ACCESSCODE, mbox.Document);
                setWindowTitle(dw.ACCESSCODE);
            }
            else if (dw.ShouldExecute && dw.ACCESSCODE == "")
            {
                MessageBox.Show("Incorrect URL!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviewExec(object sender, ExecutedRoutedEventArgs e)
        {
            InitiatePreview();
        }

        private void UndoExec(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationCommands.Undo.Execute(null, null);
        }

        private void RedoExec(object sender, ExecutedRoutedEventArgs e)
        {
            ApplicationCommands.Redo.Execute(null, null);
        }

        private void ExitProgramExec(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FindReplaceExec(object sender, ExecutedRoutedEventArgs e)
        {
            FindAndReplaceWindow win = new FindAndReplaceWindow();
            win.ShowDialog();
            if (win.OperationOKd)
            {
                mbox.findAndReplace(win.TextToFind, win.TextToReplaceWith, (win.MatchWholeWords) ? MathBox.FindAndReplaceMode.WholeWordsOnly : MathBox.FindAndReplaceMode.AnyText);
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DocumentManager.manage(mbox.Document);
            loadApplicationConfiguration();
            disableHighlightingByDefault();
            focusWorkspaceByDefault();
        }

        private void disableHighlightingByDefault()
        {
            pbar1.Visibility = Visibility.Collapsed;
            setPosition(0, 89);
        }

        private void focusWorkspaceByDefault()
        {
            this.mbox.Focus();
        }

        private void loadApplicationConfiguration()
        {
            ConfigurationManager.loadConfiguration();
        }

        private void reloadPreview(object sender)
        {
            try
            {
                LMD_Document lmdDoc = new LMD_Document(MathBox.getDocument(mbox.Document));
                pwindow.loadHTMLToPreview(lmdDoc.toHTML(LMD_Document.RenderingMode.LOCAL));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Boolean PreviewIsOpen { get; set; }

        private void InitiatePreview()
        {
            if (PreviewIsOpen)
            {
                reloadPreview(null);
                return;
            }
            try
            {
                pwindow = new PreviewWindow();
                pwindow.ReloadRequested += new ReloadRequestedHandler(reloadPreview);
                pwindow.Closed += new EventHandler(optimizator);
                LMD_Document lmdDoc = new LMD_Document(MathBox.getDocument(mbox.Document));
                pwindow.loadHTMLToPreview(lmdDoc.toHTML(LMD_Document.RenderingMode.LOCAL));
                pwindow.Show();
                PreviewIsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void optimizator(object sender, EventArgs e)
        {
            pwindow = null;
            GC.Collect();
            PreviewIsOpen = false;
        }

        private void ShowAboutMsg(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Author: Hidić Adnan, ETF Sarajevo, 2013.\nE-Mail: ahidic2@etf.unsa.ba\nThe code is open source and can be modified and distributed freely. Make sure you mention the original author! :)", "About author", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Boolean started = false;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ProgressRefresher.setProgressBarToRefresh(pbar1);
            if (started)
            {
                started = false;
                pbar1.Visibility = Visibility.Collapsed;
                bmatchingdisplay.Content = "Enable";
                mbox.HighlightsBrackets = false;
                setPosition(0, 89);
            }
            else
            {
                started = true;
                pbar1.Visibility = Visibility.Visible;
                bmatchingdisplay.Content = "Disable";
                mbox.HighlightsBrackets = true;
                setPosition(98, 187);
            }
        }

        private void setPosition(double labelPos, double buttonPos)
        {
            l_progresstext.Margin = new Thickness(labelPos, 0, 0, 0);
            bmatchingdisplay.Margin = new Thickness(buttonPos, 0, 0, 0);
        }

        private void SaveAsClicked(object sender, RoutedEventArgs e)
        {
            setWindowTitle(DocumentManager.saveAsProcedure(mbox.Document));
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!DocumentManager.handleExistingDocument(mbox.Document))
                e.Cancel = true;
        }
    }

    public static class MathEasyCommands
    {

        public static RoutedCommand NewDocument = new RoutedCommand();
        public static RoutedCommand Open = new RoutedCommand();
        public static RoutedCommand Download = new RoutedCommand();
        public static RoutedCommand Save = new RoutedCommand();
        public static RoutedCommand Upload = new RoutedCommand();
        public static RoutedCommand Preview = new RoutedCommand();
        public static RoutedCommand ExitProgram = new RoutedCommand();
        public static RoutedCommand Undo = new RoutedCommand();
        public static RoutedCommand Redo = new RoutedCommand();
        public static RoutedCommand FindAndReplace = new RoutedCommand();

        public static void InitializeCommands()
        {
            NewDocument.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            Open.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            Download.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            Save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            Upload.InputGestures.Add(new KeyGesture(Key.U, ModifierKeys.Control));
            Preview.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            ExitProgram.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));
            Redo.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            Undo.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
            FindAndReplace.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
        }
    }
}
