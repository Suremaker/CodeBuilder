using System.Reflection.Emit;
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
            Validators.PrimitiveOrReferenceType(predicate.ExpressionType, "predicate");
            _predicate = predicate;
            _thenExpression = ExprHelper.PopIfNeeded(thenExpression);
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_predicate);
            var label = ctx.DefineLabel();
            label.EmitGoto(OpCodes.Brfalse); //TODO: use Brfalse_s if possible
            ctx.Compile(_thenExpression);
            label.Mark();
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            symbolGenerator
                .Write("if (")
                .Write(_predicate)
                .Write(")")
                .WriteNamedBlock("", _thenExpression);
            return start.BlockTo(symbolGenerator.GetCurrentPosition());
        }
    }
}