using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Division : Expression
    {
        public Expression Numerator { get; set; }
        public Expression Denominator { get; set; }

        public Division(Expression Numerator, Expression Denominator)
        {
            this.Numerator = Numerator;
            this.Denominator = Denominator;
        }

        public Expression evaluate()
        {
            Expression c1 = Numerator.evaluate();
            Expression c2 = Denominator.evaluate();

            Constant const1 = c1 as Constant;
            Constant const2 = c2 as Constant;

            if (const1 != null && const2 != null)
                return new Constant(const1.Value / const2.Value);
            else
                return new Division(c1, c2);
        }

        public String toHTML()
        {
            return @"\frac {"+Numerator.toHTML()+ "} {"+Denominator.toHTML()+ "}";
        }

        public String toText()
        {
            return "( " + Numerator.toText() + " ) / ( " + Denominator.toText() + " )";
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Numerator.canEvaluateIfSymbolsConst(availableSymbols)
                && Denominator.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cNumerator = Numerator.replaceWithValue(availableSymbols);
            Expression cDenominator = Denominator.replaceWithValue(availableSymbols);

            if (cNumerator == null)
            {
                cNumerator = Numerator.cloneAndSubstitute(availableSymbols);
            }
            if (cDenominator == null)
            {
                cDenominator = Denominator.cloneAndSubstitute(availableSymbols);
            }

            return new Division(cNumerator, cDenominator);
        }

        public static String DivValidator(String input)
        {
            return (input[0] == '/') ? "/" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Numerator.symbolsInExpression(symbols);
            return Denominator.symbolsInExpression(symbols);
        }
    }
}
