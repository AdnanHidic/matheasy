using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Text.RegularExpressions;

namespace MathEasy.src.LMD.Math
{
    // todo: enable limes and integral, enable all brackets, test it
    public static class MathEngine
    {
        #region token initialization

        public static Token literalToken = new Token(Token.Type.Literal, "literal", Token.Precedence.LV1, Token.Association.LEFT, false, Literal.literalValidator);
        public static Token approachToken = new Token(Token.Type.Approach, "→", Token.Precedence.LV6, Token.Association.LEFT, false, Approach.appValidator);
        public static Token limitToken = new Token(Token.Type.Limit, "lim", Token.Precedence.LV6, Token.Association.LEFT, false, Limit.limValidator);
        public static Token subscriptToken = new Token(Token.Type.Subscript, "_", Token.Precedence.LV1, Token.Association.LEFT, false, Subscript.subValidator);

        public static Token additionToken = new Token(Token.Type.Addition, "+", Token.Precedence.LV3, Token.Association.LEFT, false, Addition.addValidator);
        public static Token subtractionToken = new Token(Token.Type.Subtraction, "-", Token.Precedence.LV3, Token.Association.LEFT, false, Subtraction.subValidator);
        public static Token symbolToken = new Token(Token.Type.Symbol, "symbol", Token.Precedence.NONE, Token.Association.NONE, false, Symbol.SymbolValidator);
        public static Token constantToken = new Token(Token.Type.Constant, "const", Token.Precedence.NONE, Token.Association.NONE, false, Constant.ConstantValidator);
        
        public static Token leftParenthesisToken = new Token(Token.Type.LeftParenthesis, "(", Token.Precedence.LV1, Token.Association.NONE, false, Bracket.ParLeftValidator);
        public static Token rightParenthesisToken = new Token(Token.Type.RightParenthesis, ")", Token.Precedence.LV1, Token.Association.NONE, false, Bracket.ParRightValidator);
        public static Token leftBraceToken = new Token(Token.Type.LeftBrace, "[", Token.Precedence.LV1, Token.Association.NONE, false, Bracket.BraceLeftValidator);
        public static Token rightBraceToken = new Token(Token.Type.RightBrace, "]", Token.Precedence.LV1, Token.Association.NONE, false, Bracket.BraceRightValidator);
        public static Token leftChevronToken = new Token(Token.Type.LeftChevron, "{", Token.Precedence.LV1, Token.Association.NONE, false, Bracket.ChevLeftValidator);
        public static Token rightChevronToken = new Token(Token.Type.RightChevron, "}", Token.Precedence.LV1, Token.Association.NONE, false, Bracket.ChevRightValidator);

        public static Token multiplicationToken = new Token(Token.Type.Multiplication, "*", Token.Precedence.LV4, Token.Association.LEFT, false, Multiplication.MultValidator);
        public static Token logarithmToken = new Token(Token.Type.Logarithm, "log", Token.Precedence.LV6, Token.Association.NONE, false, Logarithm.LogValidator);
        public static Token nlogarithmToken = new Token(Token.Type.NaturalLogarithm, "ln", Token.Precedence.LV6, Token.Association.NONE, false, NaturalLogarithm.lnValidator);
        public static Token rootToken = new Token(Token.Type.Root, "root", Token.Precedence.LV6, Token.Association.RIGHT, false, Root.RootValidator);
        public static Token divisionToken = new Token(Token.Type.Division, "/", Token.Precedence.LV4, Token.Association.LEFT, false, Division.DivValidator);
        public static Token exponentiationToken = new Token(Token.Type.Exponentiation, "^",Token.Precedence.LV5,Token.Association.RIGHT,false,Exponentiation.expValidator);
        public static Token indefiniteIntegralToken = new Token(Token.Type.IndefiniteIntegral, "integral", Token.Precedence.LV6, Token.Association.RIGHT, false, IndefiniteIntegral.intValidator);

        private static List<Token> AvailableTokens = new List<Token>()
        {
            literalToken,               // " literal "
            subscriptToken,             // _
            limitToken,                 // lim
            approachToken,              // →
            indefiniteIntegralToken,    // integral
            logarithmToken,             // log 
            nlogarithmToken,            // ln
            exponentiationToken,        // ^
            multiplicationToken,        // *
            divisionToken,              // /
            additionToken,              // +
            subtractionToken,           // -
            leftParenthesisToken,       // (
            leftBraceToken,             // [
            leftChevronToken,           // {
            rightParenthesisToken,      // )
            rightBraceToken,            // ]
            rightChevronToken,          // }
            rootToken,                  // root

            symbolToken,                // sym
            constantToken               // const
        };

        #endregion

        #region generalized token for infix to prefix

        public class OperandToken
        {
            public List<Token> Tokens { get; set; }

            public OperandToken(Token token)
            {
                Tokens = new List<Token>();
                Tokens.Add(token);
            }

            public OperandToken(List<Token> tokens)
            {
                Tokens = tokens;
            }

        }

        #endregion

        public static List<KeyValuePair<Symbol, Constant>> Table = new List<KeyValuePair<Symbol, Constant>>();

        public static void resetSymbolTable()
        {
            Table.Clear();
        }

        // expression can only consist out of the following:
        // constants: integers our decimal (.9 is not valid, 0.9 is!)
        // symbols: must start with a letter, other chars can be both numbers and letters- no spaces (e.g. ab6 is valid, 6ab is not)
        // no unary operators + -
        // unary operator ln, integral
        // binary operators + - * / ^ log lim
        // brackets
        // everything else is not supported as of 27.6.2013 @ hidex
        // null is returned if expression generation fails
        public static Expression getExpressionFromString(String input)
        {
            if (input.Length==0)
                return null;

            //phase 1: generate list of tokens as they are recognized. If nothing is recognized, return null
            List<Token> tokens = getTokensFromString(input);

            if (tokens == null)
                return null;

            //phase 2: order list of tokens so it is ordered in Polish notation. If the procedure fails, return null
            List<Token> polishOrder = toPolishNotation(tokens);

            if (polishOrder== null)
                return null;

//             foreach (Token t in polishOrder)
//                 MessageBox.Show(t.Text + " is of type " + t.TypeProperty);

            //phase 3: build expression tree and return it. If the procedure fails, return null
            return generateExpressionTree(ref polishOrder);
        }

        #region phase 1- string to list of tokens
        public static List<Token> getTokensFromString(String input)
        {
            if (input.Length == 0)
                return null;

            List<Token> tokens = new List<Token>();

            int i = 0;
            while (i<input.Length && (input[i] == ' ' || input[i] == '\t'))
                i++;
            input = input.Substring(i);

            //MessageBox.Show("|" + input + "|");

            while (input.Length > 0)
            {
                Token foundToken = AvailableTokens.Find(x => x.ValidatorFunction(input) != null);
                
                if (foundToken == null)
                    return null; // meaning no token was recognized -> syntax error
                else if (foundToken.Text == "symbol" || foundToken.Text == "const" || foundToken.Text=="literal")
                {
                    foundToken = foundToken.clone();
                    foundToken.Text = foundToken.ValidatorFunction(input);
                }

                //MessageBox.Show(foundToken.Text+" Type: "+foundToken.TypeProperty);

                tokens.Add(foundToken);
                input = input.Substring(foundToken.Text.Length);

                i = 0;
                while (i<input.Length && (input[i] == ' ' || input[i] == '\t'))
                    i++;
                input = input.Substring(i);
                //MessageBox.Show("|" + input + "|");
            }

            return (tokens.Count == 0) ? null : tokens;
        }
        #endregion

        #region phase 2 - list of tokens order in polish notation

        public static List<Token> toPolishNotation(List<Token> tokens)
        {
            //MessageBox.Show("Begginging polish");

            Stack<OperandToken> operands = new Stack<OperandToken>();
            Stack<Token> operators = new Stack<Token>();
            try
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    Token t = tokens[i];

                    int opsInBrackets = IsSpecialNaryOperator(t);
                    if (t.TypeProperty == Token.Type.Symbol || t.TypeProperty == Token.Type.Constant || t.TypeProperty == Token.Type.Literal)
                    {
                        operands.Push(new OperandToken(t));
                    }
                    else if (opsInBrackets == 1)
                    {
                        i = loadUnaryOperatorSpecial(ref operands, tokens, i);
                    }
                    else if (opsInBrackets == 2)
                    {
                        i = loadBinaryOperatorSpecial(ref operands, tokens,i);
                    }
                    else if (IsLeftBracket(t) || operators.Count == 0 || t.PrecedenceProperty > operators.Peek().PrecedenceProperty)
                    {
                        operators.Push(t);
                    }
                    else if (IsRightBracket(t))
                    {
                        while (!IsLeftBracket(operators.Peek()))
                        {
                            Token tmpToken = operators.Peek();
                            if (tmpToken.IsUnary)
                            {
                                loadUnaryOperator(ref operands, ref operators);
                            }
                            else
                            {
                                loadBinaryOperator(ref operands, ref operators);
                            }
                        }

                        OperandToken ot = operands.Pop();
                        List<Token> temp = new List<Token>();
                        Token leftBracket = operators.Pop();
                        if (!AreMatchingBrackets(leftBracket, t))
                            throw new Exception("Mistmatched brackets!");
                        
                        temp.Add(leftBracket);
                        temp.AddRange(ot.Tokens);
                        temp.Add(t);
                        ot.Tokens = temp;

                        operands.Push(ot);
                    }
                    else if (t.PrecedenceProperty <= operators.Peek().PrecedenceProperty)
                    {
                        while (operators.Count != 0 && t.PrecedenceProperty <= operators.Peek().PrecedenceProperty)
                        {
                            Token tmpToken = operators.Peek();
                            if (tmpToken.IsUnary)
                            {
                                loadUnaryOperator(ref operands, ref operators);
                            }
                            else
                            {
                                loadBinaryOperator(ref operands, ref operators);
                            }
                        }
                        operators.Push(t);
                    }
                }

                while (operators.Count != 0)
                {
                    Token tmpToken = operators.Peek();
                    if (tmpToken.IsUnary)
                    {
                        loadUnaryOperator(ref operands, ref operators);
                    }
                    else
                    {
                        loadBinaryOperator(ref operands, ref operators);
                    }
                }
            }
            catch (Exception)
            {
//                MessageBox.Show(e.Message);
                return null;
            }

//          MessageBox.Show("ending polish");
            return (operands.Count==1)?operands.Pop().Tokens:null;
            
        }

        private static int IsSpecialNaryOperator(Token t)
        {
            if (t.TypeProperty == Token.Type.Logarithm || t.TypeProperty == Token.Type.Limit || t.TypeProperty == Token.Type.IndefiniteIntegral)
                return 2;
            else if (t.TypeProperty == Token.Type.NaturalLogarithm)
                return 1;
            else
                return -1;
        }

        private static Boolean IsConstOrSymbol(Token t)
        {
            return t.TypeProperty == Token.Type.Symbol || t.TypeProperty == Token.Type.Constant;
        }

        private static Boolean IsLeftBracket(Token t)
        {
            return t.TypeProperty == Token.Type.LeftParenthesis ||
                t.TypeProperty == Token.Type.LeftBrace ||
                t.TypeProperty == Token.Type.LeftChevron;
        }

        private static Boolean IsRightBracket(Token t)
        {
            return t.TypeProperty == Token.Type.RightParenthesis ||
                t.TypeProperty == Token.Type.RightChevron ||
                t.TypeProperty == Token.Type.RightBrace;
        }

        private static Boolean AreMatchingBrackets(Token left, Token right)
        {
            if (left.TypeProperty == Token.Type.LeftParenthesis)
                return right.TypeProperty == Token.Type.RightParenthesis;
            else if (left.TypeProperty == Token.Type.LeftBrace)
                return right.TypeProperty == Token.Type.RightBrace;
            else if (left.TypeProperty == Token.Type.LeftChevron)
                return right.TypeProperty == Token.Type.RightChevron;
            else
                return false;
        }

        private static void loadUnaryOperator(ref Stack<OperandToken> operands, ref Stack<Token> operators)
        {
            Token t = operators.Pop();
            OperandToken otSingle = operands.Pop();
            otSingle.Tokens.Insert(0, t);
            operands.Push(otSingle);
        }

        private static void loadBinaryOperator(ref Stack<OperandToken> operands, ref Stack<Token> operators)
        {
            Token t = operators.Pop();
            OperandToken otRight = operands.Pop();
            OperandToken otLeft = operands.Pop();
            otLeft.Tokens.Insert(0, t);
            otLeft.Tokens.AddRange(otRight.Tokens);
            operands.Push(otLeft);
        }

        private static int loadUnaryOperatorSpecial(ref Stack<OperandToken> operands, List<Token> tokens, int position)
        {
            Stack<Token> brackets = new Stack<Token>();

            int startingIndexFirstParenthesis = -1;
            int endingIndexFirstParenthesis = -1;

            // first token after nary operator must be left parenthesis
            if (position + 1 >= tokens.Count || tokens[position + 1].TypeProperty != Token.Type.LeftParenthesis)
                throw new Exception("First token after nary operator must be a left parenthesis");
            startingIndexFirstParenthesis = position + 1;

            brackets.Push(leftParenthesisToken.clone());
            for (int i = startingIndexFirstParenthesis + 1; i < tokens.Count; i++)
            {
                if (IsLeftBracket(tokens[i]))
                    brackets.Push(tokens[i]);
                else if (IsRightBracket(tokens[i]))
                {
                    if (!AreMatchingBrackets(brackets.Pop(), tokens[i]))
                        throw new Exception("Mismatched parentheses");
                    else
                    {
                        if (brackets.Count == 0)
                        {
                            endingIndexFirstParenthesis = i;
                            break;
                        }
                    }
                }
            }

            

            if (startingIndexFirstParenthesis == -1 || endingIndexFirstParenthesis == -1 )
                throw new Exception("Starting or ending values have not been determined");

            //MessageBox.Show(tokens.Count + "\n" + startingIndexFirstParenthesis + " " + endingIndexFirstParenthesis + "\n" + startingIndexSecondParenthesis + " " + endingIndexSecondParenthesis);

            List<Token> firstOperand = tokens.GetRange(startingIndexFirstParenthesis, endingIndexFirstParenthesis - startingIndexFirstParenthesis + 1);

            firstOperand = toPolishNotation(firstOperand);

            firstOperand.Insert(0, tokens[position]);

            OperandToken ot = new OperandToken(firstOperand);

            operands.Push(ot);

            //MessageBox.Show("Ending!");
            return endingIndexFirstParenthesis;
        }

        private static int loadBinaryOperatorSpecial(ref Stack<OperandToken> operands, List<Token> tokens, int position)
        {
            //MessageBox.Show("loading binary operator special");
            Stack<Token> brackets = new Stack<Token>();

            int startingIndexFirstParenthesis = -1;
            int endingIndexFirstParenthesis = -1;
            int startingIndexSecondParenthesis = -1;
            int endingIndexSecondParenthesis = -1;

            // first token after nary operator must be left parenthesis
            if (position + 1 >= tokens.Count || tokens[position + 1].TypeProperty != Token.Type.LeftParenthesis)
                throw new Exception("First token after nary operator must be a left parenthesis");
            startingIndexFirstParenthesis = position + 1;

            brackets.Push(leftParenthesisToken.clone());
            for (int i = startingIndexFirstParenthesis + 1; i < tokens.Count; i++)
            {
                if (IsLeftBracket(tokens[i]))
                    brackets.Push(tokens[i]);
                else if (IsRightBracket(tokens[i]))
                {
                    if (!AreMatchingBrackets(brackets.Pop(), tokens[i]))
                        throw new Exception("Mismatched parentheses");
                    else
                    {
                        if (brackets.Count == 0)
                        {
                            endingIndexFirstParenthesis = i;
                            break;
                        }
                    }
                }
            }

            if (endingIndexFirstParenthesis+ 1 >= tokens.Count || tokens[endingIndexFirstParenthesis + 1].TypeProperty != Token.Type.LeftParenthesis)
                throw new Exception("Second operand of an nary operator must start with a left parenthesis");
            startingIndexSecondParenthesis = endingIndexFirstParenthesis+ 1;

            brackets.Push(leftParenthesisToken.clone());
            for (int i = startingIndexSecondParenthesis + 1; i < tokens.Count; i++)
            {
                if (IsLeftBracket(tokens[i]))
                    brackets.Push(tokens[i]);
                else if (IsRightBracket(tokens[i]))
                {
                    if (!AreMatchingBrackets(brackets.Pop(), tokens[i]))
                        throw new Exception("2- mismatched parentheses");
                    else
                    {
                        if (brackets.Count == 0)
                        {
                            endingIndexSecondParenthesis = i;
                            break;
                        }
                    }
                }
            }

            if (startingIndexFirstParenthesis == -1 || startingIndexSecondParenthesis == -1 || endingIndexFirstParenthesis == -1 || endingIndexSecondParenthesis == -1)
                throw new Exception("Starting or ending values have not been determined");

            //MessageBox.Show(tokens.Count + "\n" + startingIndexFirstParenthesis + " " + endingIndexFirstParenthesis + "\n" + startingIndexSecondParenthesis + " " + endingIndexSecondParenthesis);

            List<Token> firstOperand = tokens.GetRange(startingIndexFirstParenthesis, endingIndexFirstParenthesis - startingIndexFirstParenthesis+1);
            List<Token> secondOperand = tokens.GetRange(startingIndexSecondParenthesis, endingIndexSecondParenthesis - startingIndexSecondParenthesis+1);

            firstOperand = toPolishNotation(firstOperand);
            secondOperand = toPolishNotation(secondOperand);

            firstOperand.Insert(0,tokens[position]);
            firstOperand.AddRange(secondOperand);

            OperandToken ot = new OperandToken(firstOperand);

            operands.Push(ot);

            //MessageBox.Show("Ending!");
            return endingIndexSecondParenthesis;
        }

        #endregion

        #region phase 3 - list of tokens ordered in polish notation to expression tree
        public static Expression generateExpressionTree(ref List<Token> tokens)
        {
            Token t = tokens[0];
            tokens.RemoveAt(0);
            // constant
            if (t.TypeProperty == Token.Type.Constant)
            {
                if (t.Text == "∞")
                    return new Constant("∞");
                else
                    return new Constant(Double.Parse(t.Text.Replace(".", ",")));
            }
            // symbol
            else if (t.TypeProperty == Token.Type.Symbol)
                return new Symbol(t.Text);
            // literal
            else if (t.TypeProperty == Token.Type.Literal)
                return new Literal(t.Text);
            // addition
            else if (t.TypeProperty == Token.Type.Addition)
            {
                Expression leftOperand = generateExpressionTree(ref tokens);
                Expression rightOperand = generateExpressionTree(ref tokens);
                return new Addition(leftOperand, rightOperand);
            }
            // subtraction
            else if (t.TypeProperty == Token.Type.Subtraction)
            {
                Expression leftOperand = generateExpressionTree(ref tokens);
                Expression rightOperand = generateExpressionTree(ref tokens);
                return new Subtraction(leftOperand, rightOperand);
            }
            // division
            else if (t.TypeProperty == Token.Type.Division)
            {
                Expression numerator = generateExpressionTree(ref tokens);
                Expression denominator = generateExpressionTree(ref tokens);
                return new Division(numerator, denominator);
            }
            // multiplication
            else if (t.TypeProperty == Token.Type.Multiplication)
            {
                Expression leftOperand = generateExpressionTree(ref tokens);
                Expression rightOperand = generateExpressionTree(ref tokens);
                return new Multiplication(leftOperand, rightOperand);
            }
            // exponentiation
            else if (t.TypeProperty == Token.Type.Exponentiation)
            {
                Expression leftOperand = generateExpressionTree(ref tokens);
                Expression rightOperand = generateExpressionTree(ref tokens);
                return new Exponentiation(leftOperand, rightOperand);
            }
            // root
            else if (t.TypeProperty == Token.Type.Root)
            {
                Expression e1 = generateExpressionTree(ref tokens);
                Expression e2 = generateExpressionTree(ref tokens);
                return new Root(e1, e2);
            }
            // n logarithm
            else if (t.TypeProperty == Token.Type.NaturalLogarithm)
            {
                Expression e1 = generateExpressionTree(ref tokens);
                return new NaturalLogarithm(e1);
            }
            // logarithm
            else if (t.TypeProperty == Token.Type.Logarithm)
            {
                Expression e1 = generateExpressionTree(ref tokens);
                Expression e2 = generateExpressionTree(ref tokens);
                return new Logarithm(e1, e2);
            }
            // approach
            else if (t.TypeProperty == Token.Type.Approach)
            {
                Expression e1 = generateExpressionTree(ref tokens);
                Expression e2 = generateExpressionTree(ref tokens);
                return new Approach(e1, e2);
            }
            // limit
            else if (t.TypeProperty == Token.Type.Limit)
            {
                Expression e1 = generateExpressionTree(ref tokens);
                Expression e2 = generateExpressionTree(ref tokens);
                return new Limit(e1, e2);
            }
            // integral
            else if (t.TypeProperty == Token.Type.IndefiniteIntegral)
            {
                Expression e1 = generateExpressionTree(ref tokens);
                Expression e2 = generateExpressionTree(ref tokens);
                return new IndefiniteIntegral(e1, e2);
            }
            else if (t.TypeProperty == Token.Type.Subscript)
            {
                Expression e1 = generateExpressionTree(ref tokens);
                Expression e2 = generateExpressionTree(ref tokens);
                return new Subscript(e1, e2);
            }
            // brackets
            else if (IsLeftBracket(t))
            {
                Expression content = generateExpressionTree(ref tokens);
                tokens.RemoveAt(0);
                // ()
                if (t.TypeProperty == Token.Type.LeftParenthesis)
                {
                    return new Bracket(content, Bracket.TypeOfBracket.Parentheses);
                }
                // []
                else if (t.TypeProperty == Token.Type.LeftBrace)
                {
                    return new Bracket(content, Bracket.TypeOfBracket.Braces);
                }
                // {}
                else
                {
                    return new Bracket(content, Bracket.TypeOfBracket.Chevrons);
                }
            }
           // if we are lucky enough, we will never reach this point
           return null;
        }


        #endregion

        /// <summary>
        /// Transforms text given as input to corresponding LaTeX and HTML. 
        /// Side effects: if symbol is declared, it will automatically be identified and saved for further use.
        /// </summary>
        /// <param name="input">Input plaintext</param>
        /// <returns>Returns the input string if content as a whole could not be transformed or an error occured.</returns>
        public static String getHTML(String input)
        {
            String workingInput = input.Trim(new Char[]{' ', '$'});

            String[] partsOfInput = workingInput.Split('=');
            Expression[] expressions = partsOfInput.Select(x=> getExpressionFromString(x)).ToArray();
            if (expressions.Count(x => x == null) != 0)
                return input;

            addSymbolToTable(expressions);
            
            StringBuilder sb = new StringBuilder();
            foreach (Expression ex in expressions)
            {
                sb.Append(ex.toHTML());
                sb.Append(" = ");
            }

            workingInput = sb.ToString().TrimEnd(new Char[] { '=', ' ' });

            return @"\("+ workingInput + @"\)";
        }

        private static void addSymbolToTable(Expression[] expressions)
        {
            Symbol[] symbols = Table.Select(x => x.Key).ToArray();

            if (expressions[0] is Symbol && expressions.Length > 1 && expressions[expressions.Length - 1].canEvaluateIfSymbolsConst(symbols))
            {
                Table.Select(x => x.Key).ToArray();
                int indexIfExists = Table.FindIndex(y => y.Key.Name == (expressions[0] as Symbol).Name);

                Constant evaluated = expressions[expressions.Length - 1].evaluate().cloneAndSubstitute(Table).evaluate() as Constant;

                if (evaluated == null)
                    return;

                KeyValuePair<Symbol, Constant> newTableEntry = new KeyValuePair<Symbol, Constant>(expressions[0] as Symbol, evaluated);

                if (indexIfExists == -1)
                    Table.Add(newTableEntry);
                else
                {
                    Table.Insert(indexIfExists + 1, newTableEntry);
                    Table.RemoveAt(indexIfExists);
                }
            }
        }

        /// <summary>
        /// Searches input string for commands such as # or ?. If command is found, it is evaluated and result is returned.
        /// Side effects: if symbol is declared, it will automatically be identified and saved for further use.
        /// </summary>
        /// <param name="input">Input plaintext</param>
        /// <returns>Result of command execution or null if no command was detected</returns>
        public static String getCommandResult(String input)
        {
//             foreach (KeyValuePair<Symbol, Constant> cs in Table)
//             {
//                 MessageBox.Show("Symbol: " + cs.Key.Name + "\nValue: " + cs.Value.Value);
//             }

            String workingInput = input.Trim(new Char[] { ' ', '$' });

            String[] partsOfInput = workingInput.Split('=');

            String lastElement = partsOfInput[partsOfInput.Length - 1].Trim();

            if (lastElement == "?" && partsOfInput.Length>1)
            {
                Expression expressionToEvaluate = getExpressionFromString(partsOfInput[partsOfInput.Length - 2]);
                if (expressionToEvaluate == null)
                    return null;
                else
                {
                    expressionToEvaluate = expressionToEvaluate.cloneAndSubstitute(Table);
                    return expressionToEvaluate.evaluate().toText();
                }
            }
            else if (lastElement == "#" && partsOfInput.Length > 1)
            {
                return partsOfInput[partsOfInput.Length - 2];
            }
            else
            {
                Expression[] expressions = partsOfInput.Select(x => getExpressionFromString(x)).ToArray();
                if (expressions.Count(x => x == null) != 0)
                    return null;

                addSymbolToTable(expressions);

                return null;
            }
        }
    }
}
