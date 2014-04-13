using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class IfThenExpression : VoidExpression
    {
        private readonly Expression _predicate;
        private readonly Expression _thenExpression;

        public IfThenExpression(Expression predicate, Expression thenExpression)
        {
            Validators.NullCheck(predicate, "predicate");
            Validators.NullCheck(thenExpression, "thenExpression");
            Validators.PrimitiveOrClassCheck(predicate.ExpressionType, "predicate");
            _predicate = predicate;
            _thenExpression = ExprHelper.PopIfNeeded(thenExpression);
        }

        internal override void Compile(IBuildContext ctx)
        {
            _predicate.Compile(ctx);
            var label = ctx.Generator.DefineLabel();
            ctx.Generator.Emit(OpCodes.Brfalse, label); //TODO: use Brfalse_s if possible
            _thenExpression.Compile(ctx);
            ctx.Generator.MarkLabel(label);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append(".if (");
            _predicate.Dump(builder);
            builder.AppendLine(")").AppendLine("{");
            _thenExpression.Dump(builder);
            return builder.AppendLine().AppendLine("}");
        }
    }
}