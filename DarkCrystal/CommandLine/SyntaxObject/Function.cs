
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;

namespace DarkCrystal.CommandLine
{
    public class Function : SyntaxObject
    {
        private MethodInfo MethodInfo;
        private object Instance;

        public Function(MethodInfo methodInfo, object instance, Token token) : base(token)
        {
            this.Instance = instance;
            this.MethodInfo = methodInfo;
        }

        public Value Call(Value[] arguments, CommandLine commandLine)
        {
            CheckTypes(arguments);

            if (commandLine.FakeExecution)
            {
                return new Value(MethodInfo.ReturnType, null, Token);
            }
            else
            {
                var objectArguments = new object[arguments.Length];
                for (int i = 0; i < arguments.Length; i++)
                {
                    objectArguments[i] = arguments[i].Get();
                }
                var result = MethodInfo.Invoke(Instance, objectArguments);
                return new Value(MethodInfo.ReturnType, result, Token);
            }
        }

        public Type[] GetArgumentTypes()
        {
            var parameters = MethodInfo.GetParameters();
            var argumentTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                argumentTypes[i] = parameters[i].ParameterType;
            }
            return argumentTypes;
        }

        public override string ToString()
        {
            return String.Format("Function '{0}'", MethodInfo.Name);
        }

        private void CheckTypes(Value[] arguments)
        {
            var parameters = MethodInfo.GetParameters();
            var count = Math.Min(parameters.Length, arguments.Length);
            for (int i = 0; i < count; i++)
            {
                var argument = arguments[i];
                var parameterType = parameters[i].ParameterType;
                if (!CommandLine.EnsureType(argument, parameterType))
                {
                    var format = "Parameter {0} of function {1} expects type {2}, got {3}";
                    throw new TokenException(String.Format(format, i + 1, MethodInfo.Name, parameterType.Name, argument.Type.Name), Token);
                }
            }

            if (parameters.Length != arguments.Length)
            {
                var format = "Function {0} expects {1} parameters, got {2}";
                throw new TokenException(String.Format(format, MethodInfo.Name, parameters.Length, arguments.Length), Token);
            }
        }
    }
}