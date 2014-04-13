using CodeBuilder.Expressions;

namespace CodeBuilder.Helpers
{
    public static class ExprHelper
    {
        public static Expression PopIfNeeded(Expression expression)
        {
            return (expression.ExpressionType != typeof (void)) ? Expr.Pop(expression) : expression;
        }
    }
}
