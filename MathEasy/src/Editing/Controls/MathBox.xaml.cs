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

using MathEasy.src.Editing.Logic;
using MathEasy.src.LMD.Core;
using MathEasy.src.ConfigurationManagement;

using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Windows.Markup;

namespace MathEasy.src.Editing.Controls
{
    public partial class MathBox : RichTextBox
    {
        public MathBox()
        {
            InitializeComponent();
            this.TextChanged += this.TextChangedHandler;
            this.KeyUp += new KeyEventHandler(KeyUpHandler);
            SyntaxHighlighter.mBox = this;
            currentPosition = Document.ContentStart;
            HighlightsBrackets = false;
            AutocompleteManager.connectToAutocomplete(this);
            EditingCommands.ToggleUnderline.InputGestures.Clear();
        }

        public AutocompletePopup apopup { get; set; }


        void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.RightAlt))
                return;

            Char c = KeyDecoder.GetCharFromKey(e.Key);
            if (c.ToString() == "")
                return;

            TextPointer tEnd = CaretPosition;
            TextPointer tStart = tEnd.GetPositionAtOffset(-1);
            if (tStart == null || tEnd == null)
                return;
            else
                new TextRange(tStart, tEnd).ApplyPropertyValue(Run.ForegroundProperty, Brushes.Black);
        }

        private void CannotExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == EditingCommands.ToggleUnderline)
                MathEasy.MathEasyCommands.Upload.Execute(null,null);
            e.CanExecute = false;
            e.Handled = true;
        }

        private void CustomDeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Selection.IsEmpty;
            e.Handled = true;
        }

        private void CustomPasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Clipboard.ContainsImage() && Clipboard.ContainsText(TextDataFormat.UnicodeText);
            e.Handled = true;
        }

//         public void CustomPaste()
//         {
//             CustomPaste(null, null);
//         }

        private void CustomPaste(object sender, ExecutedRoutedEventArgs e)
        {
            this.Selection.Text= "";

            String text = Clipboard.GetText(TextDataFormat.UnicodeText);
            Document.SetDocumentWithNewlines(text);
//             String[] lines = text.Split(new String[] { Environment.NewLine,"\n","\r","\r\n" }, StringSplitOptions.None);
//             if (lines.Length >= 1)
//             {
//                 Selection.Text = lines[0];
//                 Selection.Select(Selection.End, Selection.End);
//             }
//             for (int i = 1;i<lines.Length;i++){
//                 TextPointer tp = Selection.End.InsertLineBreak();
//                 Selection.Select(tp, tp);
//                 Selection.Text = lines[i];
//             }
//             this.Selection.Select(this.Selection.End, this.Selection.End);
        }

        private void CustomDelete(object sender, ExecutedRoutedEventArgs e)
        {
            this.Selection.Text = "";
        }

        private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            AutocompleteManager.handleKeyDown(e);
            if (e.Handled || Keyboard.IsKeyDown(Key.RightAlt))
                return;

            Char c = KeyDecoder.GetCharFromKey(e.Key);

            if (e.Key == Key.Enter)
            {
                if (!Selection.IsEmpty)
                    Selection.Text = "";

                TextPointer inserted = Selection.Start.InsertLineBreak();
                Selection.Select(inserted, inserted);

                e.Handled = true;
            }
            else if ((c=='?' || c=='#') && Selection.IsEmpty)
            {
                Selection.Text = c.ToString();
                LMD_Document lmdDoc = new LMD_Document(getDocument(Document));
                String result = lmdDoc.getCommandResult();
                Selection.Text = (result == "") ? c.ToString() : result;
                Selection.Select(Selection.End, Selection.End);
                e.Handled = true;
            }
            else if (e.Key == Key.Space)
            {
                BeginChange();
                CaretPosition = CaretPosition.GetPositionAtOffset(0, LogicalDirection.Backward);
                CaretPosition.InsertTextInRun(c.ToString());
                CaretPosition = CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                e.Handled = true;
                EndChange();
            }
        }

        public static String getDocument(FlowDocument flowDoc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Block b in flowDoc.Blocks)
            {
                Paragraph p = b as Paragraph;
                if (p != null)
                {
                    foreach (Inline i in p.Inlines)
                    {
                        Run r = i as Run;
                        LineBreak lb = i as LineBreak;
                        if (r != null)
                        {
                            sb.Append(r.Text);
                        }
                        else if (lb != null)
                        {
                            sb.Append("\n");
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static String getDocumentUni(FlowDocument flowDoc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Block b in flowDoc.Blocks)
            {
                Paragraph p = b as Paragraph;
                if (p != null)
                {
                    foreach (Inline i in p.Inlines)
                    {
                        Run r = i as Run;
                        LineBreak lb = i as LineBreak;
                        if (r != null)
                        {
                            sb.Append(r.Text);
                        }
                        else if (lb != null)
                        {
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public TextPointer currentPosition;
        public DateTime timeOfChange = DateTime.Now;

        private void TextChangedHandler(object sender, TextChangedEventArgs e)
        {
            timeOfChange = DateTime.Now;
            currentPosition = Document.ContentStart;
        }

        private Boolean _highlightsBrackets;

        public Boolean HighlightsBrackets
        {
            get
            {
                return _highlightsBrackets;
            }
            set
            {
                _highlightsBrackets = value;
                if (_highlightsBrackets)
                {
                    SyntaxHighlighter.startHighlighting();
                }
                else
                {
                    SyntaxHighlighter.stopHighlighting();
                    new TextRange(Document.ContentStart, Document.ContentEnd).ApplyPropertyValue(Run.ForegroundProperty, Brushes.Black);
                    ProgressRefresher.setValue(100);
                }
            }
        }


        public void updateHighlight()
        {
            TextPointer currentPositionPlusOffset = currentPosition.GetPositionAtOffset(20);

            if (currentPositionPlusOffset == null)
                currentPositionPlusOffset = Document.ContentEnd;

            TextRange tr = new TextRange(currentPosition, currentPositionPlusOffset);
            if (tr.Text.Count(x => x == '(' || x == '[' || x == '{' || x == '}' || x == ']' || x == ')') == 0)
            {
                currentPosition = currentPositionPlusOffset;
                
                if (HighlightsBrackets)
                    ProgressRefresher.setValue(Document.ContentStart.GetOffsetToPosition(currentPosition) * 100.0 / Document.ContentStart.GetOffsetToPosition(Document.ContentEnd));

                return;
            }
            this.TextChanged -= this.TextChangedHandler;
            currentPosition = SyntaxHighlighter.Highlight(Document, currentPosition, currentPositionPlusOffset);
            this.TextChanged += this.TextChangedHandler;

            if (HighlightsBrackets)
                ProgressRefresher.setValue(Document.ContentStart.GetOffsetToPosition(currentPosition) * 100.0 / Document.ContentStart.GetOffsetToPosition(Document.ContentEnd));    
        }

        public void updateHighlightBrackets()
        {
            this.TextChanged -= this.TextChangedHandler;
            SyntaxHighlighter.ColorNonClosedBrackets();
            this.TextChanged += this.TextChangedHandler;
        }

        public enum FindAndReplaceMode{
            WholeWordsOnly,
            AnyText
        }
        public void findAndReplace(String find, String replacement, FindAndReplaceMode mode)
        {
            TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);

            if (mode == FindAndReplaceMode.WholeWordsOnly)
            {
                tr.Text = tr.Text.Replace(" " + find + " ", " "+replacement+" ");
            }
            else
            {
                tr.Text = tr.Text.Replace(find, replacement);
            }
        }
    }
}
