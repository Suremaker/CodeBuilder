﻿using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public abstract class ComparisonExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly Type[] _compatible = new[] { typeof(sbyte), typeof(byte), typeof(ushort), typeof(short), typeof(char), typeof(int) };
        private readonly Type[] _otherSupported = new[] { typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double) };
        private readonly Type[] _unsigned = new[] { typeof(uint), typeof(ulong) };

        protected ComparisonExpression(Expression left, Expression right)
            : base(typeof(bool))
        {
            Validators.NullCheck(left, "left");
            Validators.NullCheck(right, "right");
            ValidateCompatibility(left.ExpressionType, right.ExpressionType);
            _left = left;
            _right = right;
        }

        private void ValidateCompatibility(Type left, Type right)
        {
            if (CollectionHelper.Contains(_compatible, left) && CollectionHelper.Contains(_compatible, right))
                return;
            if (CollectionHelper.Contains(_otherSupported, left) && left == right)
                return;
            throw new ArgumentException(string.Format("Comparison of {0} and {1} is not supported. Try to cast left or right value to corresponding type.", left, right));
        }

        internal override void Compile(IBuildContext ctx)
        {
            _left.Compile(ctx);
            _right.Compile(ctx);
            ctx.Generator.Emit(GetOpCode(CollectionHelper.Contains(_unsigned, _left.ExpressionType)));
        }

        protected abstract OpCode GetOpCode(bool isUnsigned);


        internal override StringBuilder Dump(StringBuilder builder)
        {
            _left.Dump(builder);
            builder.Append(GetDumpSymbol());
            return _right.Dump(builder);
        }

        protected abstract string GetDumpSymbol();
    }
}