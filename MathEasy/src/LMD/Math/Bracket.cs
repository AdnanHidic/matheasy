using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Bracket : Expression
    {
        public enum TypeOfBracket
        {
            Parentheses,    // ()
            Braces,         // []
            Chevrons        // {}
        }

        public Expression Content { get; set; }
        public TypeOfBracket Type { get; set; }

        public Bracket(Expression Content, TypeOfBracket Type)
        {
            this.Content = Content;
            this.Type = Type;
        }

        public Expression evaluate()
        {
            return Content.evaluate();
        }

        public String toHTML()
        {
            if (Type == TypeOfBracket.Parentheses)
            {
                return @"\left(" + Content.toHTML() + @"\right)";
            }
            else if (Type == TypeOfBracket.Braces)
            {
                return @"\left[" + Content.toHTML() + @"\right]";
            }
            else
            {
                return @"\left\{" + Content.toHTML() + @"\right\}";
            }
        }

        public String toText()
        {
            if (Type == TypeOfBracket.Parentheses)
            {
                return @"( " + Content.toText() + @" )";
            }
            else if (Type == TypeOfBracket.Braces)
            {
                return @"[ " + Content.toText() + @" ]";
            }
            else
            {
                return @"{ " + Content.toText() + @" }";
            }
        }

        public bool canEvaluateIfSymbolsConst(Symbol[] availableSymbols)
        {
            return Content.canEvaluateIfSymbolsConst(availableSymbols);
        }


        public Expression cloneAndSubstitute(List<KeyValuePair<Symbol, Constant>> availableSymbols)
        {
            Expression cContent = Content.replaceWithValue(availableSymbols);

            if (cContent == null)
            {
                cContent = Content.cloneAndSubstitute(availableSymbols);
            }

            return new Bracket(cContent, Type);
        }

        public List<Symbol> symbolsInExpression(List<Symbol> symbols)
        {
            return Content.symbolsInExpression(symbols);
        }

        public static String ParLeftValidator(String input)
        {
            return (input[0] == '(') ? "(" : null;
        }

        public static String ParRightValidator(String input)
        {
            return (input[0] == ')') ? ")" : null;
        }

        public static String BraceLeftValidator(String input)
        {
            return (input[0] == '[') ? "[" : null;
        }

        public static String BraceRightValidator(String input)
        {
            return (input[0] == ']') ? "]" : null;
        }

        public static String ChevLeftValidator(String input)
        {
            return (input[0] == '{') ? "{" : null;
        }

        public static String ChevRightValidator(String input)
        {
            return (input[0] == '}') ? "}" : null;
        }
    }
}
