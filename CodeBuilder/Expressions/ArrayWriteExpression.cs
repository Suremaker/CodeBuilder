using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

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

        internal override void Compile(IBuildContext ctx)
        {
            _arrayInstance.Compile(ctx);
            _index.Compile(ctx);
            EmitHelper.ConvertToNativeInt(ctx, _index.ExpressionType);
            _value.Compile(ctx);

            if (ExpressionType == typeof(sbyte))
                ctx.Generator.Emit(OpCodes.Stelem_I1);
            else if (ExpressionType == typeof(short))
                ctx.Generator.Emit(OpCodes.Stelem_I2);
            else if (ExpressionType == typeof(int))
                ctx.Generator.Emit(OpCodes.Stelem_I4);
            else if (ExpressionType == typeof(long))
                ctx.Generator.Emit(OpCodes.Stelem_I8);
            else if (ExpressionType == typeof(float))
                ctx.Generator.Emit(OpCodes.Stelem_R4);
            else if (ExpressionType == typeof(double))
                ctx.Generator.Emit(OpCodes.Stelem_R8);
            else
                ctx.Generator.Emit(OpCodes.Stelem, _elementType);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            _arrayInstance.Dump(builder);
            builder.Append(" [");
            _index.Dump(builder);
            return builder.Append("]");
        }
    }
}