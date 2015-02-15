using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathEasy.src.LMD.Core;
using System.Text.RegularExpressions;

namespace MathEasy.src.LMD.Math
{
    public class Literal : Expression
    {
        public String Value { get; set; }

        public Literal(String ValueIn)
        {
            this.Value = ValueIn;
        }

        public Expression evaluate()
        {
            return new Literal(Value);
        }

        public String toHTML()
        {
            return HTMLizer.prepareForHTML(Value.Trim(new Char[]{'"'}));
        }

        public String toText()
        {
            return @""" " + Value.Trim(new Char[] { '"' }) + @" """;
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return true;
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            return new Literal(Value);
        }

        public static String literalValidator(String input)
        {
            Regex regex = new Regex(@"^""([^""]+)""");
            return (regex.IsMatch(input)) ? regex.Match(input).Value : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            return symbols;
        }
    }
}
