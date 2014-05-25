using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
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
            Validators.PrimitiveOrReferenceType(predicate.ExpressionType, "predicate");

            if (thenExpression.ExpressionType != typeof(void))
                Validators.HierarchyCheck(elseExpression.ExpressionType, thenExpression.ExpressionType, "Else expression type {0} is not assignable to then expression type {1}", "elseExpression");
            _predicate = predicate;
            _thenExpression = thenExpression;
            _elseExpression = (thenExpression.ExpressionType == typeof(void) && elseExpression.ExpressionType != typeof(void)) ? Expr.Pop(elseExpression) : elseExpression;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            var trueLabel = ctx.DefineLabel();
            var endLabel = ctx.DefineLabel();

            ctx.Compile(_predicate);
            trueLabel.EmitGoto(OpCodes.Brtrue); //TODO: use Brtrue_s if possible

            ctx.Compile(_elseExpression);
            endLabel.EmitGoto(OpCodes.Br); //TODO: use Br_s if possible

            trueLabel.Mark();
            ctx.Compile(_thenExpression);

            endLabel.Mark();
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append(".if (");
            _predicate.Dump(builder);
            builder.AppendLine(")").AppendLine("{");
            _thenExpression.Dump(builder);
            builder.AppendLine("}").AppendLine("else").AppendLine("{");
            _elseExpression.Dump(builder);
            return builder.AppendLine("}");
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            if (ExpressionType == typeof(void))
            {
                symbolGenerator
                    .Write("if (")
                    .Write(_predicate)
                    .Write(") ")
                    .Write(_thenExpression)
                    .Write("else ")
                    .Write(_elseExpression);
            }
            else
            {
                symbolGenerator
                    .Write("(")
                    .Write(_predicate)
                    .Write(") ? ")
                    .Write(_thenExpression)
                    .Write(" : ")
                    .Write(_elseExpression);
            }
            return start.BlockTo(symbolGenerator.GetCurrentPosition());
        }
    }
}