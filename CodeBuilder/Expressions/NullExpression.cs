﻿using System;
using System.Reflection.Emit;
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

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Ldnull);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.Write("null").GetCurrentPosition());
        }
    }
}