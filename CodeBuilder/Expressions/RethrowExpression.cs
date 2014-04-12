﻿using System;
using System.Reflection.Emit;
using System.Text;

namespace CodeBuilder.Expressions
{
    public class RethrowExpression:VoidExpression
    {
        internal override void Compile(IBuildContext ctx)
        {
            if(!ctx.IsInCatchBlock)
                throw new InvalidOperationException("Unable to rethrow - not in catch block.");
            ctx.Generator.Emit(OpCodes.Rethrow);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendLine(".rethrow;");
        }
    }
}