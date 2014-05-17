using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    public class LoopBreakExpression : VoidExpression
    {
        internal override void Compile(IBuildContext ctx)
        {
            var data = ctx.GetLoopData();
            if (data == null)
                throw new InvalidOperationException("Break expression can be used only inside loop");
            if (ctx.IsInFinallyBlock)
                throw new InvalidOperationException("Break expression in finally block is forbidden");
            if (ctx.IsInValueBlock)
                throw new InvalidOperationException("Break expression is forbidden in value blocks");
            if (ctx.IsInExceptionBlock)
                throw new NotSupportedException("Break expression in try-catch blocks is not supported");
            ctx.Generator.Emit(OpCodes.Br, data.BreakLabel);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendLine(".break;");
        }
    }
}