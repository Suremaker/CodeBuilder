using System;
using System.Reflection.Emit;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    public class RethrowExpression : VoidExpression
    {
        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            if (!ctx.IsInScope<CatchScope>())
                throw new InvalidOperationException("Unable to rethrow - not in catch block.");
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Rethrow);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.WriteStatementEnd("rethrow;"));
        }
    }
}