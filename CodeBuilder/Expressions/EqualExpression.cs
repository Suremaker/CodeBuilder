using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class EqualExpression : Expression
    {
        private readonly Type[] _compatible = new[] { typeof(sbyte), typeof(byte), typeof(ushort), typeof(short), typeof(char), typeof(int) };

        private readonly Expression _left;
        private readonly Expression _right;

        public EqualExpression(Expression left, Expression right)
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
            if (Validators.IsPrimitiveOrEnum(left))
            {
                if (left != right)
                    throw new ArgumentException(string.Format("Comparison of {0} and {1} is not supported. Try to cast left or right value to the same type as other.", left, right));
                return;
            }
            if (left.IsValueType || right.IsValueType)
                throw new ArgumentException("Comparison of value types is not supported.");
            if (left == right)
                return;
            if (Validators.IsInHierarchy(right, left) || Validators.IsInHierarchy(left, right))
                return;
            throw new ArgumentException(string.Format("Comparison of {0} and {1} is not supported. Try to cast left or right value to the same type as other.", left, right));
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_left);
            ctx.Compile(_right);
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Ceq);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            return start.BlockTo(symbolGenerator
                .Write(_left)
                .Write(" == ")
                .Write(_right)
                .GetCurrentPosition());
        }
    }
}