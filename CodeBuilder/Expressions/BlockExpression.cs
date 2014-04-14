﻿using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class BlockExpression : VoidExpression
    {
        private readonly Expression[] _expressions;

        public BlockExpression(Expression[] expressions)
        {
            Validators.NullCollectionElementsCheck(expressions, "expressions");
            _expressions = new Expression[expressions.Length];
            for (int i = 0; i < expressions.Length; i++)
                _expressions[i] = ExprHelper.PopIfNeeded(expressions[i]);
        }

        internal override void Compile(IBuildContext ctx)
        {
            foreach (var expression in _expressions)
                expression.Compile(ctx);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendLine("{");
            foreach (var expression in _expressions)
                expression.Dump(builder);
            return builder.AppendLine("}");
        }
    }
}