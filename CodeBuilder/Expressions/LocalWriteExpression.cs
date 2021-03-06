﻿using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class LocalWriteExpression : VoidExpression
    {
        private readonly LocalVariable _variable;
        private readonly Expression _value;

        public LocalWriteExpression(LocalVariable variable, Expression value)
        {
            Validators.NullCheck(variable, "variable");
            Validators.NullCheck(value, "value");
            Validators.HierarchyCheck(value.ExpressionType, variable.VariableType, "Unable to assign {0} to variable of type {1}", "value");
            _variable = variable;
            _value = value;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_value);
            ctx.MarkSequencePointFor(expressionId);
            EmitWriteLocal(ctx, ctx.GetLocal(_variable));
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            return start.BlockTo(symbolGenerator.Write(_variable.Name).Write(" = ").Write(_value).Write(";").GetCurrentPosition());
        }

        internal static void EmitWriteLocal(IBuildContext ctx, LocalBuilder local)
        {
            if (local.LocalIndex == 0) ctx.Generator.Emit(OpCodes.Stloc_0);
            else if (local.LocalIndex == 1) ctx.Generator.Emit(OpCodes.Stloc_1);
            else if (local.LocalIndex == 2) ctx.Generator.Emit(OpCodes.Stloc_2);
            else if (local.LocalIndex == 3) ctx.Generator.Emit(OpCodes.Stloc_3);
            else if (local.LocalIndex < 256) ctx.Generator.Emit(OpCodes.Stloc_S, local);
            else ctx.Generator.Emit(OpCodes.Stloc, local);
        }
    }
}