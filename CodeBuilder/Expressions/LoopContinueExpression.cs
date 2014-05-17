using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    public class LoopContinueExpression : VoidExpression
    {
        internal override void Compile(IBuildContext ctx)
        {
            var data = ctx.GetLoopData();
            if (data == null)
                throw new InvalidOperationException("Continue expression can be used only inside loop");
            data.ContinueLabel.EmitGoto(OpCodes.Br, ValidateJump);
        }

        private void ValidateJump(Scope jumpFrom, Scope jumpTo)
        {
            jumpFrom.ValidateJumpOutTo(jumpTo, ScopeJumpType.Break, "Loop continue expression is forbidden in {0} scope");
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendLine(".continue;");
        }
    }
}