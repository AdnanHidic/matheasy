using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Core
{
    public static class HTMLizer
    {
        public static String prepareForHTML(String input)
        {
            // this is where the filtering of HTML occurs
            String tmp = tmp = input.Replace("&", "&amp;"); // ampersand
            tmp = tmp.Replace("  ", " &nbsp;"); // multiple spaces
            tmp = tmp.Replace("<", "&lt;"); // tag opening
            tmp = tmp.Replace("\"", "&quot;"); // quotes
            tmp = tmp.Replace(">", "&gt;"); // tag closing
            tmp = tmp.Replace("\n", "<br/>"); // newlines
            return tmp;
        }
    }
}
