using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

namespace CodeBuilder.Expressions
{
    public class ArrayWriteExpression : VoidExpression
    {
        private readonly Expression _arrayInstance;
        private readonly Expression _index;
        private readonly Expression _value;
        private readonly Type _elementType;

        internal ArrayWriteExpression(Expression arrayInstance, Expression index, Expression value)
        {
            Validators.NullCheck(index, "index");
            Validators.IntegralCheck(index.ExpressionType, "Array element access requires parameter of integral type, got: {0}", "index");
            _elementType = Validators.ArrayCheck(arrayInstance, "arrayInstance").ExpressionType.GetElementType();
            Validators.NullCheck(value, "value");
            Validators.HierarchyCheck(value.ExpressionType, _elementType, "Value expression of type {0} does not match to type: {1}", "value");
            _arrayInstance = arrayInstance;
            _index = index;
            _value = value;
        }

        private void EmitArrayAccess(IBuildContext ctx)
        {
            if (ExpressionType == typeof (sbyte))
                ctx.Generator.Emit(OpCodes.Stelem_I1);
            else if (ExpressionType == typeof (short))
                ctx.Generator.Emit(OpCodes.Stelem_I2);
            else if (ExpressionType == typeof (int))
                ctx.Generator.Emit(OpCodes.Stelem_I4);
            else if (ExpressionType == typeof (long))
                ctx.Generator.Emit(OpCodes.Stelem_I8);
            else if (ExpressionType == typeof (float))
                ctx.Generator.Emit(OpCodes.Stelem_R4);
            else if (ExpressionType == typeof (double))
                ctx.Generator.Emit(OpCodes.Stelem_R8);
            else
                ctx.Generator.Emit(OpCodes.Stelem, _elementType);
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_arrayInstance);

            ctx.Compile(_index);
            EmitHelper.ConvertToNativeInt(ctx, _index.ExpressionType);

            ctx.Compile(_value);

            ctx.MarkSequencePointFor(expressionId);

            EmitArrayAccess(ctx);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var begin = symbolGenerator.GetCurrentPosition();
            var end = symbolGenerator
                .Write(_arrayInstance)
                .Write(" [")
                .Write(_index)
                .Write("] = ")
                .Write(_value)
                .Write(";")
                .GetCurrentPosition();
            return begin.BlockTo(end);
        }
    }
}