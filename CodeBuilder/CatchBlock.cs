using System;
using CodeBuilder.Expressions;
using CodeBuilder.Helpers;

namespace CodeBuilder
{
    public class CatchBlock
    {
        /// <summary>
        /// Creates catch block for exception of exceptionType.
        /// Exception instance would be available through exceptionVariable, which could be used in catchExpression.
        /// </summary>
        /// <param name="exceptionType">Exception type.</param>
        /// <param name="exceptionVariable">Variable used to access catched exception</param>
        /// <param name="catchExpression">Catch expression.</param>
        public CatchBlock(Type exceptionType, LocalVariable exceptionVariable, Expression catchExpression)
        {
            Validators.NullCheck(exceptionType, "exceptionType");
            Validators.NullCheck(catchExpression, "catchExpression");
            Validators.NullCheck(exceptionVariable, "exceptionVariable");
            Validators.HierarchyCheck(exceptionType, typeof(Exception), "Provided type {0} has to be deriving from {1}", "exceptionType");
            Validators.HierarchyCheck(exceptionType, exceptionVariable.VariableType, "Unable to assign exception of type {0} to local of type {1}", "exceptionVariable");
            ExceptionType = exceptionType;
            PreCatchExpression = Expr.WriteLocal(exceptionVariable, new ValueOnStackExpression(exceptionType));
            CatchExpression = ExprHelper.PopIfNeeded(catchExpression);
            ExceptionVariable = exceptionVariable;
        }

        /// <summary>
        /// Creates catch block for exception of exceptionType, where exception instance is not required.
        /// </summary>
        /// <param name="exceptionType">Exception type.</param>
        /// <param name="catchExpression">Catch expression.</param>
        public CatchBlock(Type exceptionType, Expression catchExpression)
        {
            Validators.NullCheck(exceptionType, "exceptionType");
            Validators.NullCheck(catchExpression, "catchExpression");
            Validators.HierarchyCheck(exceptionType, typeof(Exception), "Provided type {0} has to be deriving from {1}", "exceptionType");
            ExceptionType = exceptionType;
            PreCatchExpression = Expr.Pop(new ValueOnStackExpression(exceptionType));
            CatchExpression = ExprHelper.PopIfNeeded(catchExpression);
        }

        /// <summary>
        /// Creates catch block for exception of any type.
        /// </summary>
        /// <param name="catchExpression">Catch expression.</param>
        public CatchBlock(Expression catchExpression)
        {
            Validators.NullCheck(catchExpression, "catchExpression");
            ExceptionType = typeof (object);
            PreCatchExpression = Expr.Pop(new ValueOnStackExpression(ExceptionType));
            CatchExpression = ExprHelper.PopIfNeeded(catchExpression);
        }

        public LocalVariable ExceptionVariable { get; private set; }
        public Expression CatchExpression { get; private set; }
        public Type ExceptionType { get; private set; }
        public Expression PreCatchExpression { get; private set; }
    }
}