
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace DarkCrystal.CommandLine
{
    public class CommandLine
    {
        private readonly Lexer Lexer;
        private readonly IGlobalObjectResolver GlobalResolver;

        public bool FakeExecution { get; private set; }
        
        public CommandLine(IGlobalObjectResolver globalResolver)
        {
            this.Lexer = new Lexer();
            this.GlobalResolver = globalResolver;
        }

        public object Execute(string line, Type expectedResultType = null)
        {
            this.FakeExecution = false;
            return ExecuteInternal(line, expectedResultType);
        }

        public bool Validate(string line, out string errorText, Type expectedResultType = null)
        {
            try
            {
                if (!String.IsNullOrEmpty(line))
                {
                    this.FakeExecution = true;
                    ExecuteInternal(line, expectedResultType);
                }
                errorText = null;
                return true;
            }
            catch (Exception exception)
            {
                errorText = exception.Message;
                return false;
            }
        }

        public string AutoComplete(string line)
        {
            this.FakeExecution = true;
            try
            {
                ExecuteInternal(line + '…');
            }
            catch (AutoCompleteException exception)
            {
                return exception.AutoCompletedText;
            }
            catch (Exception exception)
            {
                exception.ToString();
            }
            return String.Empty;
        }

        public static bool EnsureType(Value argument, Type expectedType)
        {
            if (argument.Type == expectedType || argument.Type.IsSubclassOf(expectedType))
            {
                return true;
            }
            else if (argument.Type == typeof(void) && expectedType.IsClass)
            {
                argument.Convert(expectedType, null);
                return true;
            }
            else if (argument.Type == typeof(float) && expectedType == typeof(int))
            {
                var value = argument.Get();
                if (value != null)
                {
                    value = (int)(float)(value);
                }
                argument.Convert(typeof(int), value);
                return true;
            }
            else if (argument.Type == typeof(int) && expectedType == typeof(float))
            {
                var value = argument.Get();
                if (value != null)
                {
                    value = (float)(int)(value);
                }
                argument.Convert(typeof(float), value);
                return true;
            }
            return false;
        }

        private object ExecuteInternal(string line, Type expectedResultType = null)
        {
            var tokens = Lexer.Tokenize(line);
            try
            {
                tokens.MoveNext();
                var result = EvaluateExpression(tokens);
                if (tokens.Current.Type != TokenType.EndOfLine)
                {
                    throw new UnexpectedTokenException("end of line", tokens.Current);
                }

                if (expectedResultType != null)
                {
                    if (!EnsureType(result, expectedResultType))
                    {
                        var format = "Expected result type is {0}, got {1}";
                        throw new Exception(String.Format(format, expectedResultType.Name, result.Type.Name));
                    }
                }

                return result.Get();
            }
            catch (AutoCompleteException exception)
            {
                throw exception;
            }
            catch (TokenException exception)
            {
                throw new FormattedException(exception.Message, line, exception.Token);
            }
            catch (Exception exception)
            {
                throw new FormattedException(exception.Message, line, tokens.Current);
            }
        }

        private Value EvaluateExpression(IEnumerator<Token> tokens)
        {
            var operand = EvaluateOperand(tokens);
            if (tokens.Current.IsEndExpressionToken())
            {
                return operand;
            }
            else if (tokens.Current.Type == TokenType.Operator)
            {
                var operands = new List<Value> { operand };
                var operators = new List<Operator> { };
                var operatorTokens = new List<Token> { };
                while (tokens.Current.Type == TokenType.Operator)
                {
                    operators.Add(tokens.Current.Data as Operator);
                    operatorTokens.Add(tokens.Current);
                    tokens.MoveNext();
                    operands.Add(EvaluateOperand(tokens));
                }

                while (operators.Count > 1)
                {
                    int i = GetOperatorIndex(operators);
                    operands[i] = operators[i].Evaluate(operands[i], operands[i + 1], operatorTokens[i], this);
                    operands.RemoveAt(i + 1);
                    operators.RemoveAt(i);
                    operatorTokens.RemoveAt(i);
                }

                return operators[0].Evaluate(operands[0], operands[1], operatorTokens[0], this);
            }
            else
            {
                throw new UnexpectedTokenException("end of expression", tokens.Current);
            }
        }
        
        private Value EvaluateOperand(IEnumerator<Token> tokens)
        {
            switch (tokens.Current.Type)
            {
                case TokenType.Value:
                    var @value = (Value)tokens.Current.Data;
                    tokens.MoveNext();
                    return @value;

                case TokenType.OpenBracket:
                    tokens.MoveNext();
                    var content = EvaluateExpression(tokens);
                    if (tokens.Current.Type != TokenType.ClosedBracket)
                    {
                        throw new UnexpectedTokenException("')'", tokens.Current);
                    }
                    tokens.MoveNext();
                    return content;

                case TokenType.Identifier:
                    return EvaluateIdentifier(tokens);

                case TokenType.Autocomplete:
                    var startText = tokens.Current.Data as string;
                    var autoCompletedText = GlobalResolver.AutoComplete(startText) ?? startText;
                    throw new AutoCompleteException(autoCompletedText.Substring(startText.Length));

                default:
                    throw new UnexpectedTokenException("start of expression", tokens.Current);
            }
        }

        private Value EvaluateIdentifier(IEnumerator<Token> tokens)
        {
            var currentObject = GlobalResolver.GetObject(tokens.Current, FakeExecution);
            tokens.MoveNext();
            while (true)
            {
                if (tokens.Current.Type == TokenType.Dot)
                {
                    tokens.MoveNext();
                    if (tokens.Current.Type == TokenType.Identifier)
                    {
                        currentObject = currentObject.GetMember(tokens.Current);
                        tokens.MoveNext();
                    }
                    else if (tokens.Current.Type == TokenType.Autocomplete)
                    {
                        var startText = tokens.Current.Data as string;
                        var autoCompletedText = currentObject.AutoCompleteMember(startText) ?? startText;
                        throw new AutoCompleteException(autoCompletedText.Substring(startText.Length));
                    }
                    else
                    {
                        throw new UnexpectedTokenException("identifier", tokens.Current);
                    }
                }
                else if (tokens.Current.Type == TokenType.OpenBracket)
                {
                    var function = currentObject as Function;
                    if (function == null)
                    {
                        new TokenException("Try to call non-function object", tokens.Current);
                    }
                    tokens.MoveNext();
                    currentObject = EvaluateFunctionCall(function, tokens);                    
                }
                else if (tokens.Current.Type == TokenType.AssignmentOperator)
                {
                    var field = currentObject as Property;
                    if (field == null)
                    {
                        new TokenException("Try to assign non-field object", tokens.Current);
                    }
                    tokens.MoveNext();
                    currentObject = field.SetValue(EvaluateExpression(tokens), FakeExecution);
                }
                else
                {
                    return currentObject.GetValue();
                }
            }
        }

        private Value EvaluateFunctionCall(Function function, IEnumerator<Token> tokens)
        {
            if (tokens.Current.Type == TokenType.ClosedBracket)
            {
                tokens.MoveNext();
                return function.Call(EmptyArray<Value>.Value, this);
            }
            var arguments = new List<Value>();
            while (true)
            {
                arguments.Add(EvaluateExpression(tokens));
                if (tokens.Current.Type == TokenType.Comma)
                {
                    tokens.MoveNext();
                    continue;
                }
                else if (tokens.Current.Type == TokenType.ClosedBracket)
                {
                    tokens.MoveNext();
                    return function.Call(arguments.ToArray(), this);
                }
                else
                {
                    throw new UnexpectedTokenException("'('", tokens.Current);
                }
            }
        }

        private int GetOperatorIndex(List<Operator> operators)
        {
            int index = 0;
            int highestPriority = operators[index].Priority;
            for (int i = 1; i < operators.Count; i++)
            {
                int priority = operators[i].Priority;
                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    index = i;
                }
            }
            return index;
        }
    }
}