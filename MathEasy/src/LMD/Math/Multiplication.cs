using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Multiplication : Expression
    {
        public Expression LeftOperand { get; set; }
        public Expression RightOperand { get; set; }

        public Multiplication(Expression LeftOperand, Expression RightOperand)
        {
            this.LeftOperand = LeftOperand;
            this.RightOperand = RightOperand;
        }

        public Expression evaluate()
        {
            Expression c1 = LeftOperand.evaluate();
            Expression c2 = RightOperand.evaluate();

            Constant const1 = c1 as Constant;
            Constant const2 = c2 as Constant;

            if (const1 != null && const2 != null)
                return new Constant(const1.Value * const2.Value);
            else
                return new Multiplication(c1, c2);

        }

        public String toHTML()
        {
            return LeftOperand.toHTML() + @" \cdot " + RightOperand.toHTML();
        }

        public String toText()
        {
            return LeftOperand.toText() + " * " + RightOperand.toText();
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

            return new Multiplication(cLeft, cRight);
        }

        public static String MultValidator(String input)
        {
            return (input[0] == '*') ? "*" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = LeftOperand.symbolsInExpression(symbols);
            return RightOperand.symbolsInExpression(symbols);
        }
    }
}
