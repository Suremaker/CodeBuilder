﻿using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class NullExpression : Expression
    {
        internal NullExpression(Type type)
            : base(Validators.NullCheck(type, "type"))
        {
            Validators.ReferenceTypeCheck(type, "type");
        }

        internal override void Compile(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Ldnull);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.Append("null");
        }
    }
}