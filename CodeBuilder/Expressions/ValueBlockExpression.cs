﻿using System;
using System.Collections.Generic;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ValueBlockExpression : Expression
    {
        private readonly Expression[] _expressions;

        internal ValueBlockExpression(Type valueType, Expression[] expressions)
            : base(valueType)
        {
            Validators.NullCollectionElementsCheck(expressions, "expressions");
            ValidateResultType(valueType, expressions);

            _expressions = new Expression[expressions.Length];
            for (int i = 0; i < expressions.Length; i++)
                _expressions[i] = (i + 1 < expressions.Length) ? ExprHelper.PopIfNeeded(expressions[i]) : expressions[i];
        }

        private void ValidateResultType(Type valueType, Expression[] expressions)
        {
            if (expressions.Length == 0)
                throw new ArgumentException("Expected at least one expression", "expressions");
            var lastExpressionType = expressions[expressions.Length - 1].ExpressionType;
            if (!Validators.IsInHierarchy(lastExpressionType, valueType))
                throw new ArgumentException(string.Format("Expected last expression to be of {0} type, got {1}", ExpressionType, lastExpressionType));
        }

        public IEnumerable<Expression> Expressions
        {
            get { return _expressions; }
        }
        
        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            var scope = ctx.EnterScope<ValueBlockScope>();
            foreach (var expression in _expressions)
                ctx.Compile(expression);
            ctx.LeaveScope(scope);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendLine("{");
            foreach (var expression in _expressions)
                expression.Dump(builder);
            return builder.AppendLine("}");
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var begin = symbolGenerator.GetCurrentPosition();
            symbolGenerator
                .Write("{")
                .EnterScope();

            foreach (var expression in _expressions)
                symbolGenerator.Write(expression);

            var end = symbolGenerator
                .LeaveScope()
                .WriteStatementEnd("}");

            return begin.BlockTo(end);
        }
    }
}