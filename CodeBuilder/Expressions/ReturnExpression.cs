﻿using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ReturnExpression : VoidExpression
    {
        private readonly Expression _value;

        public ReturnExpression()
        {
            ReturnType = typeof(void);
            _value = null;
        }

        public ReturnExpression(Expression value)
        {
            _value = value;
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.ExpressionType == typeof(void))
                throw new ArgumentException("Void expressions cannot be returned explicitly. Please use Expr.Return() instead.");
            ReturnType = value.ExpressionType;
        }

        public Type ReturnType { get; private set; }

        internal override void Compile(IBuildContext ctx)
        {
            ValidateReturnType(ctx);
            ValidateScope(ctx);
            if (_value != null)
                _value.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Ret);
        }

        private void ValidateScope(IBuildContext ctx)
        {
            if (ctx.IsInFinallyBlock)
                throw new InvalidOperationException("Return expression is forbidden in finally blocks");
            if (ctx.IsInValueBlock)
                throw new InvalidOperationException("Return expression is forbidden in value blocks");
            if (ctx.IsInExceptionBlock)
                throw new NotSupportedException("Return expression in try-catch blocks is not supported yet");
        }

        private void ValidateReturnType(IBuildContext ctx)
        {
            Validators.HierarchyCheck(ReturnType, ctx.ReturnType, "Method return type is {0}, while return statement is returning {1}", "ReturnType");
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            if (_value == null)
                return builder.AppendLine(".return;");
            builder.Append(".return ");
            _value.Dump(builder);
            return builder.Append(";");
        }
    }
}