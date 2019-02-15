
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace DarkCrystal.CommandLine
{
    public class GlobalObjectResolver : IGlobalObjectResolver
    {
        private IGlobalObjectResolver Parent;
        private Dictionary<string, ObjectBinding> Bindings;
        
        public GlobalObjectResolver(IGlobalObjectResolver parent = null)
        {
            this.Parent = parent;
        }

        public SyntaxObject GetObject(Token token, bool fakeExecution)
        {
            var identifier = token.Data as string;
            ObjectBinding binding;
            if (Bindings.TryGetValue(identifier, out binding))
            {
                return binding.GetSyntaxObject(token, fakeExecution);
            }

            if (this.Parent != null)
            {
                return Parent.GetObject(token, fakeExecution);
            }

            throw new System.Exception(String.Format("Can't resolve global object {0}", identifier));
        }

        public virtual string AutoComplete(string startText)
        {
            if (!String.IsNullOrEmpty(startText))
            {
                if (Bindings != null)
                {
                    foreach (var name in Bindings.Keys)
                    {
                        if (name.StartsWith(startText))
                        {
                            return name;
                        }
                    }
                }
            }

            return Parent?.AutoComplete(startText);
        }
        
        public void AddClass(string name, Type type)
        {
            AddBinding(name, new ObjectBinding(BindType.Class, type, null));
        }

        public void AddValue(string name, Type type, object value)
        {
            AddBinding(name, new ObjectBinding(BindType.Value, type, value));
        }

        public void AddValueGetter(string name, Type type, Func<object> getter)
        {
            AddBinding(name, new ObjectBinding(BindType.ValueGetter, type, getter));
        }
        
        private void AddBinding(string name, ObjectBinding binding)
        {
            if (Bindings == null)
            {
                Bindings = new Dictionary<string, ObjectBinding>();
            }
            Bindings[name] = binding;
        }
    }
}