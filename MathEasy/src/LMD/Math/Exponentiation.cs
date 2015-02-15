using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Exponentiation : Expression
    {
        public Expression Base { get; set; }
        public Expression Exponent { get; set; }

        public Exponentiation(Expression Base, Expression Exponent)
        {
            this.Base = Base;
            this.Exponent = Exponent;
        }

        public Expression evaluate()
        {
            Expression c1 = Base.evaluate();
            Expression c2 = Exponent.evaluate();

            Constant const1 = c1 as Constant;
            Constant const2 = c2 as Constant;

            if (const1 != null && const2 != null)
                return new Constant(System.Math.Pow(const1.Value,const2.Value));
            else
                return new Exponentiation(c1, c2);

        }

        public String toHTML()
        {
            return Base.toHTML() + " ^ " + Exponent.toHTML();
        }

        public String toText()
        {
            return Base.toText() + " ^ " + Exponent.toText();
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Base.canEvaluateIfSymbolsConst(availableSymbols)
                && Exponent.canEvaluateIfSymbolsConst(availableSymbols);
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cLeft = Base.replaceWithValue(availableSymbols);
            Expression cRight = Exponent.replaceWithValue(availableSymbols);

            if (cLeft == null)
            {
                cLeft = Base.cloneAndSubstitute(availableSymbols);
            }
            if (cRight == null)
            {
                cRight = Exponent.cloneAndSubstitute(availableSymbols);
            }

            return new Exponentiation(cLeft, cRight);
        }

        public static String expValidator(String input)
        {
            return (input[0] == '^') ? "^" : null;
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            symbols = Base.symbolsInExpression(symbols);
            symbols = Exponent.symbolsInExpression(symbols);

            return symbols;
        }
    }
}
