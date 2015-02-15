using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathEasy.src.Editing.Controls;

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;
using MathEasy.src.ConfigurationManagement;

namespace MathEasy.src.Editing.Logic
{
    public static class AutocompleteManager
    {
        private static AutocompletePopup AutocompleteMenu = new AutocompletePopup();
        private static MathBox Editor { get; set; }
        private static TextRange CurrentContext { get; set; }

        static AutocompleteManager()
        {
            AutocompleteMenu.returnsFocus += new EventHandler(returnFocus);
            AutocompleteMenu.lb_actions.MouseDown += new MouseButtonEventHandler(returnFocus);
            AutocompleteMenu.lb_actions.MouseDoubleClick += new MouseButtonEventHandler(mouseInteractionHandler);
        }


        private static void returnFocus(object o, EventArgs e)
        {
            Editor.Focus();
        }

        private static void returnFocus(object o, MouseButtonEventArgs e)
        {
            Editor.Focus();
        }

        public static void connectToAutocomplete(MathBox mBox)
        {
            Editor = mBox;
            Editor.apopup = AutocompleteMenu;
            Editor.PreviewKeyDown += new KeyEventHandler(PreviewKeyDownHandler);
            Editor.PreviewKeyUp += new KeyEventHandler(PreviewKeyUpHandler);
            Editor.KeyUp += new KeyEventHandler(KeyUpHandler);
            Editor.SelectionChanged += new RoutedEventHandler(selectionChangedHandler);
        }

        public static void mouseInteractionHandler(object o, MouseEventArgs e)
        {
            replace();
            AutocompleteMenu.IsOpen = false;
            e.Handled = true;
            Editor.Focus();
        }

        private static void PreviewKeyUpHandler(object o, KeyEventArgs e)
        {
            if (AutocompleteMenu.IsOpen && e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Enter || e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }

        private static void PreviewKeyDownHandler(object o, KeyEventArgs e)
        {
            handleKeyDown(e);
        }

        public static void handleKeyDown(KeyEventArgs e)
        {
            if (AutocompleteMenu.IsOpen)
            {
                if (e.Key == Key.Up)
                {
                    int selectedIndex = AutocompleteMenu.lb_actions.SelectedIndex;
                    AutocompleteMenu.scrollviewer.LineUp();
                    selectedIndex = (selectedIndex == 0) ? 0 : --selectedIndex;
                    AutocompleteMenu.lb_actions.SelectedIndex = selectedIndex;

                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    int selectedIndex = AutocompleteMenu.lb_actions.SelectedIndex;
                    AutocompleteMenu.scrollviewer.LineDown();
                    selectedIndex = (selectedIndex == AutocompleteMenu.lb_actions.Items.Count - 1) ? AutocompleteMenu.lb_actions.Items.Count - 1 : ++selectedIndex;
                    AutocompleteMenu.lb_actions.SelectedIndex = selectedIndex;

                    e.Handled = true;
                }
                else if (e.Key == Key.Enter)
                {
                    replace();
                    AutocompleteMenu.IsOpen = false;

                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    AutocompleteMenu.IsOpen = false;

                    e.Handled = true;
                }
            }
        }

        private static void KeyUpHandler(object o, KeyEventArgs e)
        {
            if (Editor.Selection.IsEmpty)
                refresh();
            else
                AutocompleteMenu.IsOpen = false;
        }

        private static void selectionChangedHandler(object o, RoutedEventArgs e)
        {
            if (Editor.Selection.IsEmpty)
                refresh();
            else
                AutocompleteMenu.IsOpen = false;
        }

        private static Dictionary<String, String> findReplacementStrings(String input)
        {
            Dictionary<String, String> replacements = new Dictionary<String, String>();
            replacements = ConfigurationManager.Symbols.Where(x => x.Value.StartsWith(input.Substring(1))).ToDictionary(x => x.Key, y=>y.Value);
            return replacements;
        }

        private static TextPointer getTextPointerToWordStart(TextSelection position)
        {
            TextPointer selectionStart = position.Start;
            String s = selectionStart.GetTextInRun(LogicalDirection.Backward);

            if (position.IsEmpty)
            {
                int lastInd = s.LastIndexOf(' ');
                int offsetToPos = (lastInd == -1) ? 0 : -1;
                s = s.Substring((lastInd == -1) ? 0 : lastInd);
                TextPointer tp = selectionStart.GetPositionAtOffset(-(s.Length + offsetToPos));
                return tp;
            }
            else
            {
                return selectionStart;
            }
        }

        private static TextRange getWordStartingAtDesignatedPos(TextPointer start)
        {
            String text = start.GetTextInRun(LogicalDirection.Forward);
            int index = text.IndexOf(' ');
            index = (index == -1) ? text.Length : index;
            TextPointer zavrsetak = start.GetPositionAtOffset(index);
            return new TextRange(start, zavrsetak);
        }

        private static void refresh()
        {
            TextPointer currentWordStart = getTextPointerToWordStart(Editor.Selection);

            CurrentContext = getWordStartingAtDesignatedPos(currentWordStart);

            if (CurrentContext.Text.Length<1 || !CurrentContext.Text.StartsWith(@"\"))
            {
                AutocompleteMenu.IsOpen = false;
                return;
            }

            Dictionary<String,String> options = findReplacementStrings(CurrentContext.Text);

            if (options.Count != 0)
            {
                ListBox lb = AutocompleteMenu.lb_actions;

                lb.Items.Clear();
                foreach (KeyValuePair<String,String> a in options)
                    lb.Items.Add(a);


                Rect pos = currentWordStart.GetCharacterRect(LogicalDirection.Backward);
                AutocompleteMenu.PlacementTarget = Editor as UIElement;
                AutocompleteMenu.PlacementRectangle = pos;
                AutocompleteMenu.IsOpen = true;
                lb.SelectedIndex = 0;
                AutocompleteMenu.Focus();
            }
            else
            {
                AutocompleteMenu.IsOpen = false;
            }
        }

        private static void replace()
        {
            ListBox lb = AutocompleteMenu.lb_actions;
            KeyValuePair<String,String> pa = (KeyValuePair<String,String>)lb.SelectedItem;
            String s = CurrentContext.Text;
            CurrentContext.Text = pa.Key+" ";
            Editor.Selection.Select(CurrentContext.End, CurrentContext.End);
        }
    }
}
