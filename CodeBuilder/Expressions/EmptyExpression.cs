﻿using System.Text;

namespace CodeBuilder.Expressions
{
    public class EmptyExpression : VoidExpression
    {
        internal override void Compile(IBuildContext ctx)
        {
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder;
        }
    }
}