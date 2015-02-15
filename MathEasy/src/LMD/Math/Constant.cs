using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MathEasy.src.LMD.Math
{
    public class Constant : Expression
    {
        public Double Value { get; set; }

        public Constant(String ValueIn)
        {
            if (ValueIn == "∞")
            {
                this.Value = Double.PositiveInfinity;
            }
        }

        public Constant(Double Value)
        {
            this.Value = Value;
        }

        public Expression evaluate()
        {
            return new Constant(Value);
        }

        public String toHTML()
        {
            return (Double.IsInfinity(Value))?"∞":Value.ToString().Replace(',','.');
        }

        public String toText()
        {
            return (Double.IsInfinity(Value)) ? "∞" : Value.ToString().Replace(',', '.');
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            if (Double.IsInfinity(Value))
                return false;
            return true;
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            return new Constant(Value);
        }

        public static String ConstantValidator(String input)
        {
            Regex regex = new Regex(@"^(?:(\d+(?:\.\d+)?)|∞)");
            return (regex.IsMatch(input)) ? regex.Match(input).Value : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            return symbols;
        }
    }
}
