
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;

namespace DarkCrystal.CommandLine
{
    public class Class : BaseTypeObject
    {
        protected override BindingFlags Flags => BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod;
        protected override object Instance => null;
        
        public Class(Type @class, Token token) : base(@class, token)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} class", Type.Name);
        }
    }
}