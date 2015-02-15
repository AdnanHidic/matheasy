using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathEasy.src.LMD.Math;
using MathEasy.src.LMD.Core;

namespace MathEasy.src.LMD.Commands
{
    public static class CommandEngine
    {
        private static int count = 0;

        public static void resetCounter()
        {
            count = 0;
        }

        public static String getHTMLPlot(String input)
        {
            String workingInput = cleanInput(input);
            String[] partsOfCommand = workingInput.Split(new Char[] { ',' });

            if (partsOfCommand.Length != 4)
                return HTMLizer.prepareForHTML(input);
            try
            {
                Double lowerLimit = Double.Parse(partsOfCommand[1]);
                Double upperLimit = Double.Parse(partsOfCommand[2]);
                int numberOfPoints = int.Parse(partsOfCommand[3]);

                if (lowerLimit > upperLimit || numberOfPoints <= 0)
                    return HTMLizer.prepareForHTML(input);

                Expression expr = MathEngine.getExpressionFromString(partsOfCommand[0]);
                List<Symbol> symbolsInExpr = new List<Symbol>();
                if (expr == null || (symbolsInExpr = expr.symbolsInExpression(symbolsInExpr)).Count != 1 || !expr.canEvaluateIfSymbolsConst(symbolsInExpr.ToArray()))
                    return HTMLizer.prepareForHTML(input);

                List<KeyValuePair<Double, Double>> points = new List<KeyValuePair<Double, Double>>();
                loadPoints(ref points, expr, symbolsInExpr[0], lowerLimit,upperLimit,numberOfPoints);
                return generateHTMLPlot(points);
            }
            catch (Exception)
            {
                return HTMLizer.prepareForHTML(input);
            }
        }

        private static void loadPoints(ref List<KeyValuePair<Double, Double>> points, Expression toPlot, Symbol symbol, Double low, Double up, int num)
        {
            Double incr = (Double)(up - low) / num;
            Double y = -1;
            for (Double x = low; x <= up; x += incr)
            {
                List<KeyValuePair<Symbol,Constant>> toReplace = new List<KeyValuePair<Symbol,Constant>>(){new KeyValuePair<Symbol,Constant>(symbol,new Constant(x))};

                y = (toPlot.cloneAndSubstitute(toReplace).evaluate() as Constant).Value;
                points.Add(new KeyValuePair<double, double>(x,y));
            }
        }

        private static String generateHTMLPlot(List<KeyValuePair<Double, Double>> points)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<div id=""chart"+count+@""" style=""height:300px; width:500px;"">");
            sb.AppendLine(@"<script class=""code"" language=""javascript"" type=""text/javascript"">");
            sb.AppendLine("$(document).ready(function(){");
            sb.AppendLine("var points=[");
            for (int i = 0; i < points.Count - 1; i++)
            {
                sb.AppendLine("[" + points[i].Key.ToString().Replace(",", ".") + "," + points[i].Value.ToString().Replace(",", ".") + "],");
            }
            sb.AppendLine("[" + points[points.Count - 1].Key.ToString().Replace(",", ".") + "," + points[points.Count - 1].Value.ToString().Replace(",", ".") + "]");
            sb.AppendLine("];");
            sb.AppendLine(@"
            $.jqplot('chart"+count+@"', [points], { 
                series:[{showMarker:false}],
                axes:{
                    xaxis:{
                        label:'x'
                    },
                    yaxis:{
                        label:'y'
                    }
                }
            });
            ");

            sb.AppendLine("});");
            sb.AppendLine(@"
            </script>
            </div>");
            count++;
            return sb.ToString();
        }

        public static String getHTMLEval(String input)
        {
            String workingInput = cleanInput(input);

            Expression expressionToEval = MathEngine.getExpressionFromString(workingInput);

            if (expressionToEval == null)
                return HTMLizer.prepareForHTML(input);
            else
            {
                Expression evaluated = expressionToEval.cloneAndSubstitute(MathEngine.Table).evaluate();
                return @"\(" + evaluated.toHTML() + @"\)";
            }
        }

        private static String cleanInput(String input)
        {
            String workingInput = input.Replace(" ", "");
            workingInput = workingInput.Replace("<", "");
            workingInput = workingInput.Replace(">", "");
            workingInput = workingInput.Replace("eval", "");
            workingInput = workingInput.Replace("plot", "");
            return workingInput;
        }
    }
}
