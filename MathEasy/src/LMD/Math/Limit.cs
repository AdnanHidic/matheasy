using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Limit : Expression
    {
        public Expression Expr { get; set; }
        public Expression Lim { get; set; }

        public Limit(Expression Lim, Expression Expr)
        {
            this.Expr = Expr;
            this.Lim = Lim;
        }

        public Expression evaluate()
        {
            Expression c1 = Lim.evaluate();
            Expression c2 = Expr.evaluate();

            return new Limit(c1, c2);
        }

        public String toHTML()
        {
            return @"\lim _ { " +Lim.toHTML() +" } { " + Expr.toHTML() +" }"; 
        }

        public String toText()
        {
            return "lim ( " + Lim.toText() + " ) (" + Expr.toText() + " )";
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Expr.canEvaluateIfSymbolsConst(availableSymbols)
                && Lim.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression expr = Expr.replaceWithValue(availableSymbols);
            Expression lim = Lim.replaceWithValue(availableSymbols);

            if (expr == null)
            {
                expr = expr.cloneAndSubstitute(availableSymbols);
            }
            if (lim == null)
            {
                lim = lim.cloneAndSubstitute(availableSymbols);
            }

            return new Logarithm(lim, expr);
        }

        public static String limValidator(String input)
        {
            return (input.StartsWith("lim", StringComparison.CurrentCultureIgnoreCase)) ? "lim" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Expr.symbolsInExpression(symbols);
            return Lim.symbolsInExpression(symbols);
        }
    }
}
