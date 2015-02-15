using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class NaturalLogarithm : Expression
    {
        public Expression Base { get; set; }

        public NaturalLogarithm(Expression Base)
        {
            this.Base = Base;
        }

        public Expression evaluate()
        {
            Expression c1 = Base.evaluate();

            Constant const1 = c1 as Constant;

            if (const1 != null)
                return new Constant(System.Math.Log(const1.Value));
            else
                return new NaturalLogarithm(c1);

        }

        public String toHTML()
        {
            return @"\log _  { e } { " + Base.toHTML() + " }"; 
        }

        public String toText()
        {
            return "ln ( " + Base.toText() + " )";
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Base.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cLeft = Base.replaceWithValue(availableSymbols);

            if (cLeft == null)
            {
                cLeft = Base.cloneAndSubstitute(availableSymbols);
            }

            return new NaturalLogarithm(cLeft);
        }

        public static String lnValidator(String input)
        {
            return (input.StartsWith("ln", StringComparison.CurrentCultureIgnoreCase)) ? "ln" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Base.symbolsInExpression(symbols);

            return symbols;
        }
    }
}
