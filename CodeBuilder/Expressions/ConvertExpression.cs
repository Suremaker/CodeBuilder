using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

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

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(Expression);
            ctx.MarkSequencePointFor(expressionId);
            _conversionMethod(ctx);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            return start.BlockTo(symbolGenerator
                .Write(string.Format("({0}) ", ExpressionType.FullName))
                .Write(Expression)
                .GetCurrentPosition());
        }
    }
}