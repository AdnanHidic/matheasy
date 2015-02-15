using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Documents;
using System.Windows.Media;
using MathEasy.src.Editing.Controls;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Markup;

namespace MathEasy.src.Editing.Logic
{
    public static class SyntaxHighlighter
    {
        private static DateTime lastRequest = DateTime.Now;
        public static MathBox mBox;

        private static Thread refresher = new Thread(new ThreadStart(bworker));

        static SyntaxHighlighter()
        {
            refresher.IsBackground = true;
            _pause = true;
            refresher.Start();
        }

        public static void startHighlighting()
        {
            _pause = false;
            lock (_threadLock)
            {
                Monitor.Pulse(_threadLock);
            }
        }

        public static void stopHighlighting()
        {
            _pause = true;
        }

        private static bool _pause;
        private static object _threadLock = new object();

        private static Action a = new Action(delegate() { mBox.updateHighlight(); });
        private static Action aW = new Action(delegate() { mBox.updateHighlightBrackets(); });

        private static Boolean shouldExecuteFinal = false;

        private static void bworker()
        {
            while (true)
            {
                if (_pause)
                {
                    lock (_threadLock)
                    {
                        Monitor.Wait(_threadLock);
                    }
                }
                else if ((DateTime.Now - mBox.timeOfChange).Milliseconds < 150)
                {
                    System.Threading.Thread.Sleep(50);
                    continue;
                }
                if (mBox.currentPosition.CompareTo(mBox.Document.ContentEnd) == 0)
                {
                    if (shouldExecuteFinal)
                    {
                        mBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, aW);
                        shouldExecuteFinal = false;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                }
                else
                {
                    mBox.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, a);
                    if (mBox.currentPosition.CompareTo(mBox.Document.ContentEnd) == 0)
                        shouldExecuteFinal = true;
                    System.Threading.Thread.Sleep(30);
                }
            }
        }

        public static TextPointer Highlight(FlowDocument flowDoc, TextPointer start, TextPointer end)
        {       
            if (start.CompareTo(flowDoc.ContentStart)==0)
                brackets.Clear();

            // this is SO SLOW
            //TextRange documentRange = new TextRange(start, end);
            //documentRange.ClearAllProperties();

            TextPointer tpointer = start;
            while (tpointer.CompareTo(end)!=0)
            {
                String s = tpointer.GetTextInRun(LogicalDirection.Forward);
                TextPointer next = tpointer.GetPositionAtOffset(1);
                if (s.Length != 0)
                {
                    if (s[0] == '(' || s[0] == '[' || s[0] == '{')
                    {
                        Tag t = new Tag();
                        t.StartPosition = tpointer;
                        t.EndPosition = tpointer.GetPositionAtOffset(1);
                        t.Word = s[0].ToString();
                        brackets.Push(t);
                    }
                    else if (s[0] == ')' || s[0] == ']' || s[0] == '}')
                    {
                        if (brackets.Count > 0)
                        {
                            Tag left = brackets.Pop();
                            Boolean matched = rightVariant(left.Word) == s[0].ToString();
                            TextPointer tpS = tpointer;
                            TextPointer tpE = tpointer.GetPositionAtOffset(1);
                            colorizeBracket(left.StartPosition, left.EndPosition, matched);
                            colorizeBracket(tpS, tpE, matched);
                        }
                        else
                        {
                            colorizeBracket(tpointer, tpointer.GetPositionAtOffset(1), false);
                        }
                    }
                }
                tpointer = next;
            }

            return tpointer;
        }
        struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;

        }

        private static Stack<Tag> brackets = new Stack<Tag>();


        public static void ColorNonClosedBrackets()
        {
            while (brackets.Count > 0)
            {
                Tag t = brackets.Pop();
                colorizeBracket(t.StartPosition, t.EndPosition, false);
            }
        }

        private static String rightVariant(String leftVariant)
        {
            if (leftVariant == "(") return ")";
            else if (leftVariant == "[") return "]";
            else return "}";
        }


        private static void colorizeBracket(TextPointer bracketStart, TextPointer bracketEnd, Boolean matched)
        {
            TextRange trange = new TextRange(bracketStart, bracketEnd);

            if (matched)
            {
                trange.ApplyPropertyValue(Run.ForegroundProperty, brushOk);
            }
            else
            {
                trange.ApplyPropertyValue(Run.ForegroundProperty, brushWrong);
            }
        }

        private static Brush brushOk = new SolidColorBrush(Colors.Blue);
        private static Brush brushWrong = new SolidColorBrush(Colors.Red);
    }
}
