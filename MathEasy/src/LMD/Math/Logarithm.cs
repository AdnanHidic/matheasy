using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Logarithm : Expression
    {
        public Expression Base { get; set; }
        public Expression Power { get; set; }

        public Logarithm(Expression Base, Expression Expr)
        {
            this.Base = Base;
            this.Power = Expr;
        }

        public Expression evaluate()
        {
            Expression c1 = Base.evaluate();
            Expression c2 = Power.evaluate();

            Constant const1 = c1 as Constant;
            Constant const2 = c2 as Constant;

            if (const1 != null && const2 != null)
                return new Constant(System.Math.Log(const2.Value, const1.Value));
            else
                return new Logarithm(c1, c2);
        }

        public String toHTML()
        {
            return @"\log _ { " +Base.toHTML() +" } { " + Power.toHTML() +" }"; 
        }

        public String toText()
        {
            return "log ( " + Base.toText() + " ) (" + Power.toText() + " )";
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Base.canEvaluateIfSymbolsConst(availableSymbols)
                && Power.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cBase = Base.replaceWithValue(availableSymbols);
            Expression cPower = Power.replaceWithValue(availableSymbols);

            if (cBase == null)
            {
                cBase = Base.cloneAndSubstitute(availableSymbols);
            }
            if (cPower == null)
            {
                cPower = Power.cloneAndSubstitute(availableSymbols);
            }

            return new Logarithm(cBase, cPower);
        }

        public static String LogValidator(String input)
        {
            return (input.StartsWith("log", StringComparison.CurrentCultureIgnoreCase)) ? "log" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Base.symbolsInExpression(symbols);
            return Power.symbolsInExpression(symbols);
        }
    }
}
