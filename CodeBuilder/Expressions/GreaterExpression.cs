using System.Reflection.Emit;

namespace CodeBuilder.Expressions
{
    public class GreaterExpression : ComparisonExpression
    {
        public GreaterExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        protected override OpCode GetOpCode(bool isUnsigned)
        {
            return isUnsigned ? OpCodes.Cgt_Un : OpCodes.Cgt;
        }

        protected override string GetDumpSymbol()
        {
            return " > ";
        }
    }
}