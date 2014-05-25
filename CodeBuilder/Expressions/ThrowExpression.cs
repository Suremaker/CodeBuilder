using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
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

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_expression);
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Throw);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.Write("throw ").Write(_expression).WriteStatementEnd(";"));
        }
    }
}