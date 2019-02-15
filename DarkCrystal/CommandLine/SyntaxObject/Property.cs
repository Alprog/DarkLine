
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;

namespace DarkCrystal.CommandLine
{
    public class Property : SyntaxObject
    {
        private Type PropertyType;
        private object Instance;
        
        private Func<object> Getter;
        private Action<object> Setter;
        private string PropertyName;
        private bool IsStatic;
        
        public Property(PropertyInfo propertyInfo, object instance, Token token) : base(token)
        {
            this.Instance = instance;
            this.PropertyType = propertyInfo.PropertyType;
            this.Getter = () => propertyInfo.GetValue(instance);
            this.Setter = (value) => propertyInfo.SetValue(instance, value);
            this.PropertyName = propertyInfo.Name;
            this.IsStatic = propertyInfo.GetAccessors(true)[0].IsStatic;
        }

        public Property(FieldInfo fieldInfo, object instance, Token token) : base(token)
        {
            this.Instance = instance;
            this.PropertyType = fieldInfo.FieldType;
            this.Getter = () => fieldInfo.GetValue(instance);
            this.Setter = (value) => fieldInfo.SetValue(instance, value);
            this.PropertyName = fieldInfo.Name;
            this.IsStatic = fieldInfo.IsStatic;
        }

        public override Value GetValue()
        {
            if (!IsStatic && Instance == null)
            {
                return new Value(PropertyType, null, Token);
            }
            else
            {
                return new Value(PropertyType, Getter(), Token);
            }
        }

        public override SyntaxObject GetMember(Token token)
        {
            return GetValue().GetMember(token);
        }

        public override string AutoCompleteMember(string startText)
        {
            return GetValue().AutoCompleteMember(startText);
        }

        public Value SetValue(Value value, bool fakeExecution)
        {
            if (!CommandLine.EnsureType(value, PropertyType))
            {
                var format = "Assignment expects {0} got {1}";
                var message = String.Format(format, PropertyType.Name, value.Type.Name);
                throw new TokenException(message, Token);
            }

            if (!fakeExecution)
            {
                Setter(value.Get());
            }

            return value;
        }

        public override string ToString()
        {
            return String.Format("Property '{0}'", PropertyName);
        }
    }
}