using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.LMD.Math
{
    public class Token
    {
        public enum Type
        {
            Addition,
            Subtraction,
            Division,
            Multiplication,
            Exponentiation,
            LeftParenthesis,
            RightParenthesis,
            LeftBrace,
            RightBrace,
            LeftChevron,
            RightChevron,
            Symbol,
            Constant,
            Logarithm,
            NaturalLogarithm,
            Root,
            IndefiniteIntegral,
            Approach,
            Limit,
            Subscript,
            Literal
        }

        public enum Precedence {
            LV6=6,
            LV5=5,
            LV4=4,
            LV3=3,
            LV2=2,
            LV1=1,
            NONE = 0
        }

        public enum Association {
            NONE,
            LEFT,
            RIGHT
        }

        public String Text { get; set; }
        public Precedence PrecedenceProperty { get; set; }
        public Association AssociationProperty { get; set; }
        public Boolean IsUnary { get; set; }
        public Func<String, String> ValidatorFunction { get; set; }
        public Type TypeProperty { get; set; }

        public Token(Type TypeProperty, String Text, Precedence Precedence, Association Association, Boolean IsUnary, Func<String,String> ValidatorFunction)
        {
            this.TypeProperty = TypeProperty;
            this.Text = Text;
            this.PrecedenceProperty = Precedence;
            this.AssociationProperty = Association;
            this.IsUnary = IsUnary;
            this.ValidatorFunction = ValidatorFunction;
        }

        public Token clone()
        {
            return new Token(TypeProperty,Text, PrecedenceProperty, AssociationProperty, IsUnary, ValidatorFunction);
        }

        public Boolean isOfTypeAndText(Token t)
        {
            return TypeProperty == t.TypeProperty && Text == t.Text;
        }

        public Boolean isOfTypeAndText(Type type, String content)
        {
            return TypeProperty == type && Text == content;
        }
    }
}
