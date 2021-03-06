﻿using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ArrayReadExpression : Expression
    {
        private readonly Expression _arrayInstance;
        private readonly Expression _index;

        internal ArrayReadExpression(Expression arrayInstance, Expression index)
            : base(Validators.ArrayCheck(arrayInstance, "arrayInstance").ExpressionType.GetElementType())
        {
            Validators.NullCheck(index, "index");
            Validators.IntegralCheck(index.ExpressionType, "Array element access requires parameter of integral type, got: {0}", "index");
            _arrayInstance = arrayInstance;
            _index = index;
        }

        private void EmitArrayAccess(IBuildContext ctx)
        {
            if (ExpressionType == typeof(byte))
                ctx.Generator.Emit(OpCodes.Ldelem_U1);
            else if (ExpressionType == typeof(sbyte))
                ctx.Generator.Emit(OpCodes.Ldelem_I1);
            else if (ExpressionType == typeof(ushort) || ExpressionType == typeof(char))
                ctx.Generator.Emit(OpCodes.Ldelem_U2);
            else if (ExpressionType == typeof(short))
                ctx.Generator.Emit(OpCodes.Ldelem_I2);
            else if (ExpressionType == typeof(int))
                ctx.Generator.Emit(OpCodes.Ldelem_I4);
            else if (ExpressionType == typeof(uint))
                ctx.Generator.Emit(OpCodes.Ldelem_U4);
            else if (ExpressionType == typeof(long))
                ctx.Generator.Emit(OpCodes.Ldelem_I8);
            else if (ExpressionType == typeof(float))
                ctx.Generator.Emit(OpCodes.Ldelem_R4);
            else if (ExpressionType == typeof(double))
                ctx.Generator.Emit(OpCodes.Ldelem_R8);
            else
                ctx.Generator.Emit(OpCodes.Ldelem, ExpressionType);
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_arrayInstance);

            ctx.Compile(_index);
            EmitHelper.ConvertToNativeInt(ctx, _index.ExpressionType);

            ctx.MarkSequencePointFor(expressionId);

            EmitArrayAccess(ctx);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var location = symbolGenerator.GetCurrentPosition();
            symbolGenerator
                .Write(_arrayInstance)
                .Write(" [")
                .Write(_index)
                .Write("]");
            return location.BlockTo(symbolGenerator.GetCurrentPosition());
        }
    }
}