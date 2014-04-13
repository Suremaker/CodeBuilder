using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class LoopExpression : VoidExpression
    {
        private readonly Expression _loop;

        public LoopExpression(Expression loop)
        {
            Validators.NullCheck(loop, "loop");
            _loop = ExprHelper.PopIfNeeded(loop);
        }

        internal override void Compile(IBuildContext ctx)
        {
            var data = new LoopData(ctx.Generator.DefineLabel(), ctx.Generator.DefineLabel());
            ctx.SetLoopData(data);

            ctx.Generator.MarkLabel(data.ContinueLabel);
            _loop.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Br, data.ContinueLabel);

            ctx.Generator.MarkLabel(data.BreakLabel);
            ctx.ResetLoopData(data);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendLine(".loop").AppendLine("{");
            _loop.Dump(builder);
            return builder.AppendLine("}");
        }
    }
}