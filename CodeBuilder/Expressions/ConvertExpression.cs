using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public abstract class ConvertExpression : Expression
    {
        protected readonly Expression Expression;
        private readonly Action<IBuildContext> _conversionMethod;

        protected ConvertExpression(Expression expression, Type type)
            : base(Validators.NullCheck(type, "type"))
        {
            Validators.NullCheck(expression, "expression");
            Validators.ConversionCheck(expression.ExpressionType, type, "Cannot convert type from {0} to {1}", "expression");
            Expression = expression;
            _conversionMethod = DetermineOpCode(Expression.ExpressionType, ExpressionType);
        }

        private Action<IBuildContext> DetermineOpCode(Type castFrom, Type castTo)
        {
            if (castFrom == castTo)
                return EmitNothing;

            if (castFrom.IsValueType && !castTo.IsValueType)
                return EmitBox;
            if (castTo.IsValueType && !castFrom.IsValueType)
                return EmitUnbox;

            if (castTo.IsPrimitive && castFrom.IsPrimitive)
                return EmitConv;

            if (!castTo.IsAssignableFrom(castFrom))
                return EmitCast;

            //else no action is needed
            return EmitNothing;
        }

        internal override void Compile(IBuildContext ctx)
        {
            Expression.Compile(ctx);
            _conversionMethod(ctx);
        }

        private void EmitBox(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Box, Expression.ExpressionType);
        }

        private void EmitUnbox(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Unbox_Any, ExpressionType);
        }

        private void EmitCast(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Castclass, ExpressionType);
        }

        protected abstract void EmitConv(IBuildContext ctx);

        private void EmitNothing(IBuildContext ctx)
        {
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendFormat("({0}) ", ExpressionType);
            return Expression.Dump(builder);
        }
    }
}