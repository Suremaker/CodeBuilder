using System.Text;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    public class EmptyExpression : VoidExpression
    {
        internal override void Compile(IBuildContext ctx, int expressionId)
        {
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder;
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var positions = symbolGenerator.GetCurrentPosition();
            return positions.BlockTo(positions);
        }
    }
}