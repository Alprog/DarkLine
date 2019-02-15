
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;

namespace DarkCrystal.CommandLine
{
    public class Value : BaseTypeObject
    {
        private object m_value;

        protected override BindingFlags Flags => BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.Static |BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod;
        protected override object Instance => m_value;

        public object Get() => m_value;

        public Value(Type valueType, object m_value, Token token) : base(valueType, token)
        {
            this.m_value = m_value;
        }

        public void Convert(Type newType, object newValue)
        {
            this.Type = newType;
            this.m_value = newValue;
        }

        public override Value GetValue()
        {
            return this;
        }

        public override string ToString()
        {
            if (m_value == null)
            {
                return String.Format("{0} instance", Type.Name);
            }
            else
            {
                return m_value.ToString();
            }
        }
    }
}