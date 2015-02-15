using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Root : Expression
    {
        public Expression Degree { get; set; }
        public Expression Radicand { get; set; }

        public Root(Expression Degree, Expression Radicand)
        {
            this.Degree = Degree;
            this.Radicand = Radicand;
        }

        public Expression evaluate()
        {
            Expression c1 = Degree.evaluate();
            Expression c2 = Radicand.evaluate();

            Constant const1 = c1 as Constant;
            Constant const2 = c2 as Constant;

            if (const1 != null && const2 != null)
                return new Constant(System.Math.Pow(const2.Value,1.0/const1.Value));
            else
                return new Root(c1, c2);
        }

        public String toHTML()
        {
            return @"\sqrt [" + Degree.toHTML() + "] {" + Radicand.toHTML() + "}";
        }

        public String toText()
        {
            return Degree.toText() + " root " + Radicand.toHTML();
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Degree.canEvaluateIfSymbolsConst(availableSymbols)
                && Radicand.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cDegree = Degree.replaceWithValue(availableSymbols);
            Expression cRadicand = Radicand.replaceWithValue(availableSymbols);

            if (cDegree == null)
            {
                cDegree = Degree.cloneAndSubstitute(availableSymbols);
            }
            if (cRadicand == null)
            {
                cRadicand = Radicand.cloneAndSubstitute(availableSymbols);
            }

            return new Root(cDegree, cRadicand);
        }

        public static String RootValidator(String input)
        {
            return (input.StartsWith("root", StringComparison.CurrentCultureIgnoreCase)) ? "root" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Degree.symbolsInExpression(symbols);
            return Radicand.symbolsInExpression(symbols);
        }
    }
}
