using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ConvertExpression : Expression
    {
        private readonly Expression _expression;

        public ConvertExpression(Expression expression, Type type)
            : base(type)
        {
            Validators.NullCheck(expression, "expression");
            Validators.AssignableCheck(expression.ExpressionType, type, "Cannot convert type from {0} to {1}", "expression");
            _expression = expression;
        }

        internal override void Compile(IBuildContext ctx)
        {
            _expression.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Box, _expression.ExpressionType);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendFormat("({0}) ", ExpressionType);
            return _expression.Dump(builder);
        }
    }
}