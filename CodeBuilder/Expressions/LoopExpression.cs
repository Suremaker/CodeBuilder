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

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            var data = new LoopData(ctx.DefineLabel(), ctx.DefineLabel());
            ctx.SetLoopData(data);

            data.ContinueLabel.Mark();
            ctx.Compile(_loop);
            data.ContinueLabel.EmitGoto(OpCodes.Br);

            data.BreakLabel.Mark();
            ctx.ResetLoopData(data);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendLine(".loop").AppendLine("{");
            _loop.Dump(builder);
            return builder.AppendLine("}");
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.WriteNamedBlock("while (true)", _loop).GetCurrentPosition());
        }
    }
}