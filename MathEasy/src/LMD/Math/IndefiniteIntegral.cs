using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class IndefiniteIntegral : Expression
    {
        public Expression Expr { get; set; }
        public Expression Variables { get; set; }

        public IndefiniteIntegral(Expression Expr, Expression Variables)
        {
            this.Expr = Expr;
            this.Variables = Variables;
        }

        public Expression evaluate()
        {
            Expression c1 = Expr.evaluate();

            return new IndefiniteIntegral(c1, Variables);

        }

        public String toHTML()
        {
            return @"\int { " + Expr.toHTML() +" "+Variables.toHTML()+" }"; 
        }

        public String toText()
        {
            return "integral ( " + Expr.toText() + " ) ( "+Variables.toText()+" ) ";
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Expr.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cLeft = Expr.replaceWithValue(availableSymbols);

            if (cLeft == null)
            {
                cLeft = Expr.cloneAndSubstitute(availableSymbols);
            }

            return new IndefiniteIntegral(cLeft,Variables);
        }

        public static String intValidator(String input)
        {
            return (input.StartsWith("integral", StringComparison.CurrentCultureIgnoreCase)) ? "integral" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Expr.symbolsInExpression(symbols);

            return symbols;
        }
    }
}
