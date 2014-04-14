using System.Reflection.Emit;

namespace CodeBuilder.Expressions
{
    public class LessExpression : ComparisonExpression
    {
        public LessExpression(Expression left, Expression right)
            : base(left, right)
        {
        }

        protected override OpCode GetOpCode(bool isUnsigned)
        {
            return isUnsigned ? OpCodes.Clt_Un : OpCodes.Clt;
        }

        protected override string GetDumpSymbol()
        {
            return " < ";
        }
    }
}