using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ValueTypePointerExpression : Expression
    {
        private readonly Expression _expression;

        internal ValueTypePointerExpression(Expression expression)
            : base(Validators.NullCheck(expression, "expression").ExpressionType)
        {
            if (!expression.ExpressionType.IsValueType)
                throw new ArgumentException("Expression has to be of value type");
            _expression = expression;
        }

        internal override void Compile(IBuildContext ctx)
        {
            _expression.Compile(ctx);
            var local = ctx.Generator.DeclareLocal(_expression.ExpressionType);
            ctx.Generator.Emit(OpCodes.Stloc, local);
            ctx.Generator.Emit(OpCodes.Ldloca, local);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return _expression.Dump(builder);
        }

        protected override Expression ReturnCallableForm()
        {
            return this;
        }
    }
}