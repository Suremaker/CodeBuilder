using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class IfThenElseExpression : Expression
    {
        private readonly Expression _predicate;
        private readonly Expression _thenExpression;
        private readonly Expression _elseExpression;

        public IfThenElseExpression(Expression predicate, Expression thenExpression, Expression elseExpression)
            : base(Validators.NullCheck(thenExpression, "thenExpression").ExpressionType)
        {
            Validators.NullCheck(predicate, "predicate");
            Validators.NullCheck(elseExpression, "elseExpression");
            Validators.PrimitiveOrClassCheck(predicate.ExpressionType, "predicate");

            if (thenExpression.ExpressionType != typeof(void))
                Validators.AssignableCheck(elseExpression.ExpressionType, thenExpression.ExpressionType, "Else expression type {0} is not assignable to then expression type {1}", "elseExpression");
            _predicate = predicate;
            _thenExpression = thenExpression;
            _elseExpression = (thenExpression.ExpressionType == typeof(void) && elseExpression.ExpressionType != typeof(void)) ? Expr.Pop(elseExpression) : elseExpression;
        }

        internal override void Compile(IBuildContext ctx)
        {
            var trueLabel = ctx.Generator.DefineLabel();
            var endLabel = ctx.Generator.DefineLabel();

            _predicate.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Brtrue, trueLabel); //TODO: use Brtrue_s if possible

            _elseExpression.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Br, endLabel); //TODO: use Br_s if possible

            ctx.Generator.MarkLabel(trueLabel);
            _thenExpression.Compile(ctx);

            ctx.Generator.MarkLabel(endLabel);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append(".if (");
            _predicate.Dump(builder);
            builder.AppendLine(")").AppendLine("{");
            _thenExpression.Dump(builder);
            builder.AppendLine().AppendLine("}").AppendLine("else").AppendLine("{");
            _elseExpression.Dump(builder);
            return builder.AppendLine().AppendLine("}");
        }
    }
}