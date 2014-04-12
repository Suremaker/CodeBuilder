using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ThrowExpression : VoidExpression
    {
        private readonly Expression _expression;

        public ThrowExpression(Expression expression)
        {
            Validators.NullCheck(expression, "expression");
            Validators.HierarchyCheck(expression.ExpressionType, typeof(Exception), "Expecting {1} type while got {0}", "expression");
            _expression = expression;
        }

        internal override void Compile(IBuildContext ctx)
        {
            _expression.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Throw);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append(".throw ");
            _expression.Dump(builder);
            return builder.AppendLine(";");
        }
    }
}