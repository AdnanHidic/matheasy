using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Text.RegularExpressions;

using System.Windows;
using MathEasy.src.LMD.Math;

namespace MathEasy.src.LMD.Core
{
    public class LMD_Document
    {
        public enum RenderingMode
        {
            LOCAL,
            WEB
        }

        public List<LMD_Object> Content { get; set; }

        public static String localMathJaxPath = AppDomain.CurrentDomain.BaseDirectory + @"Dependencies\MathJax\MathJax.js";
        public static String localJqueryPath = AppDomain.CurrentDomain.BaseDirectory + @"Dependencies\jqPlot\jquery.min.js";
        public static String localJqPlotCssPath = AppDomain.CurrentDomain.BaseDirectory + @"Dependencies\jqPlot\jquery.jqplot.min.css";
        public static String localExCanvasPath = AppDomain.CurrentDomain.BaseDirectory + @"Dependencies\jqPlot\excanvas.js";
        public static String localJqPlotPath = AppDomain.CurrentDomain.BaseDirectory + @"Dependencies\jqPlot\jquery.jqplot.min.js";

        public static String localMathJax = @"file:/"+localMathJaxPath.Replace(@"\","/")+"?config=TeX-AMS_HTML-full";
        public static String localJquery = @"file:/" + localJqueryPath.Replace(@"\", "/");
        public static String localJqPlotCss = @"file:/" + localJqPlotCssPath.Replace(@"\", "/");
        public static String localExCanvas = @"file:/" + localExCanvasPath.Replace(@"\", "/");
        public static String localJqPlot = @"file:/" + localJqPlotPath.Replace(@"\", "/");


        private String headerLocale =
        @"  <!-- saved from url=(0014)about:internet -->
            <html>
            <head>
                <meta http-equiv=""content-type"" content=""text/html; charset=UTF-8"">
                <style TYPE=""text/css"">
                    body{
                        font-family:Segoe UI;
                        font-size:14px;
                    }
                </style>
                <script type=""text/x-mathjax-config"">
MathJax.Hub.Config({
    messageStyle: 'none',    
    extensions: [""tex2jax.js""],    
    ""HTML-CSS"": { minScaleAdjust: 150}    
  });   
                </script>
                <script type=""text/javascript"" src="""+localMathJax+ @"""></script>
                <script type=""text/javascript"">
                    document.oncontextmenu = function(){return false;};
                    document.onselectstart= function() {return false;}; 
                </script>
<!-- Don't touch this! -->
    <script class=""include"" type=""text/javascript"" src="""+localJquery +@"""></script>
    <link class=""include"" rel=""stylesheet"" type=""text/css"" href="""+localJqPlotCssPath+@""" />
  
    <!--[if lt IE 9]><script language=""javascript"" type=""text/javascript"" src="""+localExCanvas+@"""></script><![endif]-->
    
   
    <script class=""include"" type=""text/javascript"" src="""+localJqPlot+@"""></script>
<!-- End Don't touch this! -->
            </head>
            <body>";

        private String headerGlobal =
        @"  <html>
            <head>
                <meta http-equiv=""content-type"" content=""text/html; charset=UTF-8"">
                <style TYPE=""text/css"">
                    body{
                        font-family:Segoe UI;
                        font-size:14px;
                    }
                </style>
                <script type=""text/x-mathjax-config"">
MathJax.Hub.Config({
    messageStyle: 'none',    
    extensions: [""tex2jax.js""],    
    ""HTML-CSS"": { minScaleAdjust: 150}    
  });   
                </script>
                <script type=""text/javascript"" src=""http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS_HTML-full""></script>
<!-- Don't touch this! -->
<script class=""include"" type=""text/javascript"" src=""http://localhost/jqPlot/jquery.min.js""></script>    
<link class=""include"" rel=""stylesheet"" type=""text/css"" href=""http://localhost/jqPlot/jquery.jqplot.min.css"" />
  
    <!--[if lt IE 9]><script language=""javascript"" type=""text/javascript"" src=""http://localhost/jqPlot/excanvas.js""></script><![endif]-->
    
   
    <script class=""include"" type=""text/javascript"" src=""http://localhost/jqPlot/jquery.jqplot.min.js""></script>
<!-- End Don't touch this! -->
            </head>
            <body>";

        private const String footer = 
        @"  </body>
        </html>";

        public LMD_Document(String input)
        {
            Content = new List<LMD_Object>();
            createObjects(input);
            eliminateDuplicates();
        }

        public void eliminateDuplicates()
        {
            for (int i = 0; i < Content.Count; i++)
            {
                if (Content[i] is LMD_Plaintext && i + 1 < Content.Count && Content[i + 1] is LMD_Plaintext)
                {
                    LMD_Plaintext c1 = Content[i] as LMD_Plaintext;
                    LMD_Plaintext c2 = Content[i + 1] as LMD_Plaintext;

                    String mergedText = c1.Text + c2.Text;
                    LMD_Plaintext newPtext = new LMD_Plaintext(mergedText);

                    Content.RemoveAt(i);
                    Content.RemoveAt(i);

                    Content.Insert(i, newPtext);
                }
            }
        }

        public bool isEqualTo(LMD_Document documentToCompareWith)
        {
            // we ensure that docToCompareWith has content and its size is equal to ours' content's size
            if (documentToCompareWith.Content != null && documentToCompareWith.Content.Count != Content.Count)
                return false;

            // we check each element against the equivalent from the cmp doc
            for (int i = 0; i < Content.Count; i++)
            {
                if (!Content[i].isEqualTo(documentToCompareWith.Content[i]))
                    return false;
            }

            // if we exited the loop it means all the corresponding elements are equal
            return true;
        }

        public String toHTML(RenderingMode ModeForRendering)
        {
            MathEngine.resetSymbolTable();

            StringBuilder generatedHTML = new StringBuilder();

            generatedHTML.Append((ModeForRendering == RenderingMode.LOCAL) ? headerLocale : headerGlobal);

            foreach (LMD_Object lmdObj in Content)
            {
                // first, the object is built (generation of expression tree etc.)
                lmdObj.build();
                // then HTML is generated
                generatedHTML.Append(lmdObj.HTML);
            }

            generatedHTML.Append(footer);

            return generatedHTML.ToString();
        }

        public String getHTMLBody()
        {
            MathEngine.resetSymbolTable();

            StringBuilder generatedHTML = new StringBuilder();

            foreach (LMD_Object lmdObj in Content)
            {
                // first, the object is built (generation of expression tree etc.)
                lmdObj.build();
                // then HTML is generated
                generatedHTML.Append(lmdObj.HTML);
            }

            return generatedHTML.ToString();
        }

        public String getCommandResult()
        {
            MathEngine.resetSymbolTable();

            foreach (LMD_Object lmdObj in Content)
            {
                LMD_Mathtext mtext = lmdObj as LMD_Mathtext;
                if (mtext != null)
                {
                    String commandResult = mtext.buildAndReturnResultIfPossible();
                    if (commandResult != null)
                        return commandResult;
                }
            }
            return "";
        }

        #region private methods for document generation and manipulation

        private void createObjects(String input)
        {
            if (input.Length == 0)
            {
                Content.Add(new LMD_Plaintext(""));
                return;
            }

            while (input.Length != 0)
            {
                LMD_Object foundElement = getElementAtStartOfAString(input);
                Content.Add(foundElement);
                input = input.Substring(foundElement.Text.Length);
            }
        }

        private static LMD_Object getElementAtStartOfAString(String input)
        {
            if (input==null || input.Length == 0)
                return new LMD_Plaintext ("");

            LMD_Object lmdObj = null;
            
            // this is guaranteed to be a valid start of an element. The else part will take care of the escaped $
            if (input[0] == '$')
            {
                lmdObj = readMathtext(input);
            }
            // 7 is minimal length of a valid eval command - notice the parentheses!
            // simplest eval is eval(x) -> length is 7
            else if (input.Length>=7 && input.Substring(0, 4).ToLower() == "eval")
            {
                lmdObj = readCommandtextEval(input);
            }
            // 13 is minimal length of a valid plot command - notice the parentheses and parameters
            // simplest plot is plot(x,0,5,5) -> length is 13
            else if (input.Length>=13 && input.Substring(0, 4).ToLower() == "plot")
            {
                lmdObj = readCommandtextPlot(input);
            }

            // if no condition was satisfied, lmdObj shall be null so plaintext object will be created
            return (lmdObj == null) ? readPlaintext(input) : lmdObj;
        }

        private static LMD_Mathtext readMathtext(String input)
        {
            int endPos = -1;
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == '$' && !isPrecededBy(input, i, @"\"))
                {
                    endPos = i;
                    break;
                }
            }

            return (endPos == -1) ? null : new LMD_Mathtext(input.Substring(0, endPos + 1));
        }

        private static LMD_Plaintext readPlaintext(String input)
        {
            if (input == null || input.Length == 0)
                return new LMD_Plaintext("");

            int endPos = -1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 'e' && i+4<=input.Length-1)
                {
                    if (i!=0 && input.Substring(i, 4).ToLower() == "eval" && (input[i + 4] == ' ' || input[i + 4] == '<'))
                    {
                        endPos = i - 1;
                        break;
                    }
                }
                else if (input[i] == 'p' && i+4<=input.Length-1)
                {
                    if (i!=0 && input.Substring(i, 4).ToLower() == "plot" && (input[i + 4] == ' ' || input[i + 4] == '<'))
                    {
                        endPos = i - 1;
                        break;
                    }
                }
                else if (i!=0 && input[i] == '$' && !isPrecededBy(input, i, @"\"))
                {
                    endPos = i - 1;
                    break;
                }
                Boolean jumped = input[i] == ' ' || input[i] == '\t';
                while (i<input.Length && (input[i] == ' ' || input[i] == '\t'))  i++;
                if (jumped) i--;
            }

            String substring = (endPos==-1)?input:input.Substring(0, endPos+1);
            return new LMD_Plaintext(substring);
        }

        private static LMD_Commandtext readCommandtextPlot(String input)
        {
            Regex r = new Regex(@"^(plot\s*<[^,>]*,\s*\d+(?:.\d*)?\s*,\s*\d+(?:.\d*)?\s*,\s*\d*\s*>)");
            if (r.IsMatch(input, 0))
                return new LMD_Commandtext(r.Match(input, 0).Value,LMD_Commandtext.TypeOfCommand.Plot);
            else
                return null;
        }

        private static LMD_Commandtext readCommandtextEval(String input)
        {
            Regex r = new Regex(@"^(eval\s*<[^>]*>)");
            if (r.IsMatch(input, 0))
                return new LMD_Commandtext(r.Match(input, 0).Value, LMD_Commandtext.TypeOfCommand.Eval);
            else
                return null;
        }

        private static Boolean isPrecededBy(String input, int positionToBePreceded, String precedingString)
        {
            if (positionToBePreceded <= 0 || positionToBePreceded >= input.Length || precedingString.Length > positionToBePreceded)
                return false;
            String precedingInString = input.Substring(positionToBePreceded - precedingString.Length, precedingString.Length);
            return precedingInString == precedingString;
        }

        #endregion
    }
}
