using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public interface Expression
    {
        Boolean canEvaluateIfSymbolsConst(Symbol[] availableSymbols);
        Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols);
        Expression evaluate();
        String toText();
        String toHTML();
        List<Symbol> symbolsInExpression(List<Symbol> symbols);
    }

    public static class ExpressionExtensionMethods
    {
        public static Constant replaceWithValue(this Expression symbol, List<KeyValuePair<Symbol,Constant>> availableSymbols)
        {
            Symbol s = symbol as Symbol;
            if (s == null)
                return null;
            return availableSymbols.Find(x => x.Key.Name == s.Name).Value;
        }
    }
}
