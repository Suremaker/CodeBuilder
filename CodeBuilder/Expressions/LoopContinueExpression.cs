using System;
using System.Reflection.Emit;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    public class LoopContinueExpression : VoidExpression
    {
        private void ValidateJump(Scope jumpFrom, Scope jumpTo)
        {
            jumpFrom.ValidateJumpOutTo(jumpTo, ScopeJumpType.Break, "Loop continue expression is forbidden in {0} scope");
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            var data = ctx.GetLoopData();
            if (data == null)
                throw new InvalidOperationException("Continue expression can be used only inside loop");
            ctx.MarkSequencePointFor(expressionId);
            data.ContinueLabel.EmitGoto(OpCodes.Br, ValidateJump);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.WriteStatementEnd("continue;"));
        }
    }
}