using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

namespace CodeBuilder.Expressions
{
    public class PopExpression : VoidExpression
    {
        private readonly Expression _expression;

        public PopExpression(Expression expression)
        {
            Validators.NullCheck(expression, "expression");
            _expression = expression;
            if (expression.ExpressionType == typeof(void))
                throw new ArgumentException(string.Format("Expected expression to be non void type, but got: {0}", expression.ExpressionType), "expression");
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_expression);
            ctx.Generator.Emit(OpCodes.Pop);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.Write(_expression).Write(";").GetCurrentPosition());
        }
    }
}