﻿using System.Reflection.Emit;
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

        internal override void Compile(IBuildContext ctx)
        {
            var trueLabel = ctx.DefineLabel();
            var endLabel = ctx.DefineLabel();

            _predicate.Compile(ctx);
            trueLabel.EmitGoto(OpCodes.Brtrue); //TODO: use Brtrue_s if possible

            _elseExpression.Compile(ctx);
            endLabel.EmitGoto(OpCodes.Br); //TODO: use Br_s if possible

            trueLabel.Mark();
            _thenExpression.Compile(ctx);

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
    }
}