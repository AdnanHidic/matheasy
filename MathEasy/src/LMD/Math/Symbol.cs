using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MathEasy.src.LMD.Math
{
    public class Symbol : Expression
    {
        public String Name { get; set; }

        public Symbol(String Name)
        {
            this.Name = Name;
        }

        public Expression evaluate()
        {
            return new Symbol(Name);
        }

        public String toHTML()
        {
            return Name;
        }

        public String toText()
        {
            return Name;
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            foreach (Symbol s in availableSymbols)
            {
                if (s.Name == Name)
                    return true;
            }
            return false;
        }

        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression c= this.replaceWithValue(availableSymbols);

            if (c == null)
            {
                return new Symbol(Name);
            }

            return c;
        }

        public static String SymbolValidator(String input)
        {
            Regex regex = new Regex(@"^([\p{L}][\p{L}|\d]*)");
            return (regex.IsMatch(input)) ? regex.Match(input).Value : null;
        }


        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            if (symbols.Count(x => x.Name == Name) == 0)
                symbols.Add(this);
            return symbols;
        }
    }
}
