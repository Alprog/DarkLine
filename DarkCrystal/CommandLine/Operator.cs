
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace DarkCrystal.CommandLine
{
    public class Operator
    {
        public delegate object HandlerType(object a, object b);

        private static Dictionary<string, Operator> Operators;

        public string Name { get; protected set; }
        public HandlerType Handler { get; protected set; }
        public int Priority { get; private set; }
        public Type ArgumentType1 { get; protected set; }
        public Type ArgumentType2 { get; protected set; }
        public Type ResultType { get; protected set; }

        static Operator()
        {
            Operators = new Dictionary<string, Operator>();
            int priority = 0;
            AddOperator<object, object, object>("=", null, priority);
            priority++;
            AddOperator<bool, bool, bool>("||", LogicalOr, priority);
            priority++;
            AddOperator<bool, bool, bool>("&&", LogicalAnd, priority);
            priority++;
            AddOperator<object, object, bool>("==", EqualsTo, priority);
            AddOperator<object, object, bool>("!=", NotEqualsTo, priority);
            priority++;
            AddOperator<float, float, bool>("<", LessThan, priority);
            AddOperator<float, float, bool>(">", GreaterThan, priority);
            AddOperator<float, float, bool>("<=", LessThanOrEqualTo, priority);
            AddOperator<float, float, bool>(">=", GreaterThanOrEqualTo, priority);
            priority++;
            AddOperator<float, float, float>("+", Addition, priority);
            AddOperator<float, float, float>("-", Subtraction, priority);
            priority++;
            AddOperator<float, float, float>("*", Multiplication, priority);
            AddOperator<float, float, float>("/", Division, priority);
        }

        public Value Evaluate(Value a, Value b, Token token, CommandLine commandLine)
        {
            if (!CommandLine.EnsureType(a, ArgumentType1) || !CommandLine.EnsureType(b, ArgumentType2))
            {
                var format = "Operator {0} expects {1} and {2} got {3} and {4}";
                var message = String.Format(format, Name, ArgumentType1.Name, ArgumentType2.Name, a.Type.Name, b.Type.Name);
                throw new TokenException(message, token);
            }
 
            if (commandLine.FakeExecution)
            {
                return new Value(ResultType, null, token);
            }
            else
            {
                return new Value(ResultType, Handler(a.Get(), b.Get()), token);
            }
        }

        private Operator()
        {
        }

        public static Operator Get(string name) => Operators[name];
        
        private static void AddOperator<ArgT1, ArgT2, ResultT>(string name, HandlerType handler, int priority)
        {
            var @operator = new Operator();
            @operator.Name = name;
            @operator.Handler = handler;
            @operator.Priority = priority;
            @operator.ArgumentType1 = typeof(ArgT1);
            @operator.ArgumentType2 = typeof(ArgT2);
            @operator.ResultType = typeof(ResultT);
            Operators[name] = @operator;
        }

        private static object LogicalOr(object a, object b)
        {
            return (bool)a || (bool)b;
        }

        private static object LogicalAnd(object a, object b)
        {
            return (bool)a && (bool)b;
        }

        private static object EqualsTo(object a, object b)
        {
            return Object.Equals(a, b);
        }

        private static object NotEqualsTo(object a, object b)
        {
            return !Object.Equals(a, b);
        }

        private static object LessThan(object a, object b)
        {
            return (float)a < (float)b;
        }
        
        private static object GreaterThan(object a, object b)
        {
            return (float)a > (float)b;
        }
        
        private static object LessThanOrEqualTo(object a, object b)
        {
            return (float)a <= (float)b;
        }
        
        private static object GreaterThanOrEqualTo(object a, object b)
        {
            return (float)a >= (float)b;
        }

        private static object Addition(object a, object b)
        {
            return (float)a + (float)b;
        }

        private static object Subtraction(object a, object b)
        {
            return (float)a - (float)b;
        }

        private static object Multiplication(object a, object b)
        {
            return (float)a * (float)b;
        }

        private static object Division(object a, object b)
        {
            return (float)a / (float)b;
        }

    }
}