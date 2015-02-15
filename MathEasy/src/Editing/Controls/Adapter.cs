using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Controls;

using System.Windows;

namespace MathEasy.src.Editing.Controls
{
    public static class Adapter
    {
        public static void SetDocumentWithNewlines(this FlowDocument flowDoc, String s)
        {
            RichTextBox mb = flowDoc.Parent as RichTextBox;

            mb.Selection.Text = "";

            String[] lines = s.Split(new String[] { Environment.NewLine, "\n", "\r", "\r\n" }, StringSplitOptions.None);
            if (lines.Length >= 1)
            {
                mb.Selection.Text = lines[0];
                mb.Selection.Select(mb.Selection.End, mb.Selection.End);
            }
            for (int i = 1; i < lines.Length; i++)
            {
                TextPointer tp = mb.Selection.End.InsertLineBreak();
                mb.Selection.Select(tp, tp);
                mb.Selection.Text = lines[i];
            }
            mb.Selection.Select(mb.Selection.End, mb.Selection.End);
        }
    }
}
