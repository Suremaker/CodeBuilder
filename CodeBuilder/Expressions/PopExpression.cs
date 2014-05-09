﻿using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class PopExpression : VoidExpression
    {
        private readonly Expression _expression;

        public PopExpression(Expression expression)
        {
            Validators.NullCheck(expression, "expression");
            _expression = expression;
            if (expression.ExpressionType == typeof(void))
                throw new ArgumentException(string.Format("Expected expression to be non void type, but got: {0}", expression.ExpressionType), "expression");
        }

        internal override void Compile(IBuildContext ctx)
        {
            _expression.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Pop);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return _expression.Dump(builder).AppendLine(";");
        }
    }
}