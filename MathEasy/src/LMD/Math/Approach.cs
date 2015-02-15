using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Approach : Expression
    {
         public Expression LeftOperand { get; set; }
        public Expression RightOperand { get; set; }

        public Approach(Expression LeftOperand, Expression RightOperand)
        {
            this.LeftOperand = LeftOperand;
            this.RightOperand = RightOperand;
        }

        public Expression evaluate()
        {
            Expression c1 = LeftOperand.evaluate();
            Expression c2 = RightOperand.evaluate();

            return new Approach(c1, c2);

        }

        public String toHTML()
        {
            return LeftOperand.toHTML() + " → " + RightOperand.toHTML();
        }

        public String toText()
        {
            return LeftOperand.toText() + " → " + RightOperand.toText();
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return LeftOperand.canEvaluateIfSymbolsConst(availableSymbols)
                && RightOperand.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cLeft = LeftOperand.replaceWithValue(availableSymbols);
            Expression cRight = RightOperand.replaceWithValue(availableSymbols);

            if (cLeft == null)
            {
                cLeft = LeftOperand.cloneAndSubstitute(availableSymbols);
            }
            if (cRight == null)
            {
                cRight = RightOperand.cloneAndSubstitute(availableSymbols);
            }

            return new Addition(cLeft, cRight);
        }

        public static String appValidator(String input)
        {
            return (input[0] == '→') ? "→" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = LeftOperand.symbolsInExpression(symbols);
            symbols = RightOperand.symbolsInExpression(symbols);

            return symbols;
        }
    }
}
