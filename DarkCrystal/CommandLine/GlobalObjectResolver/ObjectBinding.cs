
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.CommandLine
{
    public struct ObjectBinding
    {
        public BindType BindType;
        public Type Type;
        public object Value;

        public ObjectBinding(BindType bindType, Type type, object value)
        {
            this.BindType = bindType;
            this.Type = type;
            this.Value = value;
        }
        
        public SyntaxObject GetSyntaxObject(Token token, bool fakeExecution)
        {
            switch (BindType)
            {
                case BindType.Class:
                    return new Class(Type, token);

                case BindType.Value:
                    return new Value(Type, fakeExecution ? null : Value, token);

                case BindType.ValueGetter:
                    var value = fakeExecution ? null : ((Func<object>)Value)();
                    return new Value(Type, value, token);
                    
                default:
                    throw new Exception("Unknown bind type");
            }
        }
    }
}