using System;
using System.Text;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class TryFinallyExpression : VoidExpression
    {
        private readonly Expression _tryExpression;
        private readonly Expression _finallyExpression;

        public TryFinallyExpression(Expression tryExpression, Expression finallyExpression)
        {
            Validators.NullCheck(tryExpression, "tryExpression");
            Validators.NullCheck(finallyExpression, "finallyExpression");
            _tryExpression = (tryExpression.ExpressionType != typeof(void)) ? Expr.Pop(tryExpression) : tryExpression;
            _finallyExpression = (finallyExpression.ExpressionType != typeof(void)) ? Expr.Pop(finallyExpression) : finallyExpression;
        }

        internal override void Compile(IBuildContext ctx)
        {
            var label = ctx.Generator.BeginExceptionBlock();
            ctx.SetExceptionBlock(label);

            _tryExpression.Compile(ctx);

            ctx.Generator.BeginFinallyBlock();
            ctx.SetFinallyBlock(label);
            _finallyExpression.Compile(ctx);

            ctx.ResetFinallyBlock(label);
            ctx.ResetExceptionBlock(label);
            ctx.Generator.EndExceptionBlock();
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendLine(".try").AppendLine("{");
            _tryExpression.Dump(builder);
            builder.AppendLine(";").AppendLine("}").AppendLine(".finally").AppendLine("{");
            _finallyExpression.Dump(builder);
            return builder.AppendLine(";").AppendLine("}");
        }
    }
}