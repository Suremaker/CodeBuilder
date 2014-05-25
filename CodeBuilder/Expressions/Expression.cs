using System;
using System.IO;
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
        internal abstract void Compile(IBuildContext ctx, int expressionId);
        internal abstract CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator);

        public override string ToString()
        {
            var builder = new StringBuilder();
            WriteDebugCode(new MethodSymbolGenerator(null, new StringWriter(builder)));
            return builder.ToString();
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
