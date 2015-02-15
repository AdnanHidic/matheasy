using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.Editing.Controls
{
    public static class ProgressRefresher
    {
        private static System.Windows.Controls.ProgressBar pbar;

        public static void setProgressBarToRefresh(System.Windows.Controls.ProgressBar progressBar)
        {
            pbar = progressBar;
            pbar.Minimum = 0;
            pbar.Maximum = 100;
        }

        public static void setValue(double val)
        {
            if (pbar!=null) pbar.Value = val;
        }
    }
}
