using System;
using CodeBuilder.Context;
using CodeBuilder.Expressions;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

namespace CodeBuilder
{
    public class CatchBlock
    {
        private readonly Expression _catchExpression;
        private readonly Expression _preCatchExpression;
        public Type ExceptionType { get; private set; }

        /// <summary>
        /// Creates catch block for exception of exceptionType.
        /// Exception instance would be available through exceptionVariable, which could be used in catchExpression.
        /// </summary>
        /// <param name="exceptionType">Exception type.</param>
        /// <param name="exceptionVariable">Variable used to access caught exception</param>
        /// <param name="declareVariable">If true, exception variable would be declared before use</param>
        /// <param name="catchExpression">Catch expression.</param>
        public CatchBlock(Type exceptionType, LocalVariable exceptionVariable, bool declareVariable, Expression catchExpression)
        {
            Validators.NullCheck(exceptionType, "exceptionType");
            Validators.NullCheck(catchExpression, "catchExpression");
            Validators.NullCheck(exceptionVariable, "exceptionVariable");
            Validators.HierarchyCheck(exceptionType, typeof(Exception), "Provided type {0} has to be deriving from {1}", "exceptionType");
            Validators.HierarchyCheck(exceptionType, exceptionVariable.VariableType, "Unable to assign exception of type {0} to local of type {1}", "exceptionVariable");
            ExceptionType = exceptionType;
            _preCatchExpression = new ExceptionCatchInitializerExpression(exceptionType, exceptionVariable, declareVariable);
            _catchExpression = ExprHelper.PopIfNeeded(catchExpression);
        }
        public CatchBlock(Type exceptionType, LocalVariable exceptionVariable, Expression catchExpression)
            : this(exceptionType, exceptionVariable, true, catchExpression) { }
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
            _preCatchExpression = new ExceptionCatchInitializerExpression(exceptionType, null, false);
            _catchExpression = ExprHelper.PopIfNeeded(catchExpression);
        }

        /// <summary>
        /// Creates catch block for exception of any type.
        /// </summary>
        /// <param name="catchExpression">Catch expression.</param>
        public CatchBlock(Expression catchExpression)
        {
            Validators.NullCheck(catchExpression, "catchExpression");
            ExceptionType = typeof(object);
            _preCatchExpression = new ExceptionCatchInitializerExpression(ExceptionType, null, false);
            _catchExpression = ExprHelper.PopIfNeeded(catchExpression);
        }

        internal void Compile(IBuildContext ctx)
        {
            ctx.Generator.BeginCatchBlock(ExceptionType);
            var scope = ctx.EnterScope<CatchScope>();
            ctx.Compile(_preCatchExpression);
            ctx.Compile(_catchExpression);
            ctx.LeaveScope(scope);
        }

        internal void WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            symbolGenerator
                .Write(_preCatchExpression)
                .WriteNamedBlock("", _catchExpression);
        }
    }
}