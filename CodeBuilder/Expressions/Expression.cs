using System;
using System.Text;

namespace CodeBuilder.Expressions
{
    public abstract class Expression
    {
        protected Expression(Type expressionType)
        {
            ExpressionType = expressionType;
        }

        public Type ExpressionType { get; private set; }
        internal abstract void Compile(IBuildContext ctx);
        internal abstract StringBuilder Dump(StringBuilder builder);

        public override string ToString()
        {
            return Dump(new StringBuilder()).ToString();
        }
    }
}
