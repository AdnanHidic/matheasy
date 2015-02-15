using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Subscript: Expression
    {
        public Expression Base { get; set; }
        public Expression Sub { get; set; }

        public Subscript(Expression Base, Expression Sub)
        {
            this.Base = Base;
            this.Sub = Sub;
        }

        public Expression evaluate()
        {
            Expression c1 = Base.evaluate();
            Expression c2 = Sub.evaluate();

            return new Subscript(c1, c2);

        }

        public String toHTML()
        {
            return Base.toHTML() + " _ { " + Sub.toHTML() + " }";
        }

        public String toText()
        {
            return Base.toText() + " _ " + Sub.toText();
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Base.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cLeft = Base.replaceWithValue(availableSymbols);
            Expression cRight = Sub.replaceWithValue(availableSymbols);

            if (cLeft == null)
            {
                cLeft = Base.cloneAndSubstitute(availableSymbols);
            }
            if (cRight == null)
            {
                cRight = Sub.cloneAndSubstitute(availableSymbols);
            }

            return new Subscript(cLeft, cRight);
        }

        public static String subValidator(String input)
        {
            return (input[0] == '_') ? "_" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Base.symbolsInExpression(symbols);
            symbols = Sub.symbolsInExpression(symbols);

            return symbols;
        }
    }
}
