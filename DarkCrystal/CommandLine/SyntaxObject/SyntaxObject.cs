
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.CommandLine
{
    public abstract class SyntaxObject
    {
        protected Token Token;

        public SyntaxObject(Token token)
        {
            this.Token = token;
        }

        public virtual Value GetValue()
        {
            throw new TokenException(String.Format("{0} can't be resolved as value", ToString()), Token);
        }

        public virtual SyntaxObject GetMember(Token token)
        {
            throw new TokenException(String.Format("Can't get member {0} from {1}", token.Data as string, ToString()), token);
        }

        public virtual string AutoCompleteMember(string startText)
        {
            return null;
        }
    }
}