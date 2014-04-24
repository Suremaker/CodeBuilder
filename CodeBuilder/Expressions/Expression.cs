using System;
using System.Text;
using CodeBuilder.Context;

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

        internal Expression EnsureCallableForm()
        {
            if (ExpressionType == typeof(void))
                throw new InvalidOperationException("Void expression cannot be made callable");
            return ReturnCallableForm();
        }

        protected virtual Expression ReturnCallableForm()
        {
            if (!ExpressionType.IsValueType)
                return this;
            return new ValueTypePointerExpression(this);
        }
    }
}
