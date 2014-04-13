using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    public class PopExpression : VoidExpression
    {
        private readonly Expression _expression;

        public PopExpression(Expression expression)
        {
            _expression = expression;
            if (expression.ExpressionType == typeof(void))
                throw new ArgumentException("Expected expression to be non void type, but got void", "expression");
        }

        internal override void Compile(IBuildContext ctx)
        {
            _expression.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Pop);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return _expression.Dump(builder).AppendLine(";");
        }
    }
}