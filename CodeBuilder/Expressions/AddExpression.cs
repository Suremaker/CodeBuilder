using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class AddExpression : Expression
    {
        private readonly Type[] _compatible = new[] { typeof(sbyte), typeof(byte), typeof(ushort), typeof(short), typeof(char), typeof(int), typeof(uint) };
        private readonly Type[] _otherCompatible = new[] { typeof(ulong), typeof(long) };
        private readonly Type[] _overflowIgnorant = new[] { typeof(float), typeof(double) };
        private readonly Type[] _overflowUnsigned = new[] { typeof(uint), typeof(ulong) };
        private readonly Expression _left;
        private readonly Expression _right;
        private readonly bool _overflowCheck;

        public AddExpression(Expression left, Expression right, bool overflowCheck = false)
            : base(Validators.NullCheck(left, "left").ExpressionType)
        {
            Validators.NullCheck(right, "right");
            ValidateCompatibility(left.ExpressionType, right.ExpressionType);
            _left = left;
            _right = right;
            _overflowCheck = overflowCheck;
        }

        private void ValidateCompatibility(Type left, Type right)
        {
            if (CollectionHelper.Contains(_compatible, left) && CollectionHelper.Contains(_compatible, right))
                return;
            if (CollectionHelper.Contains(_otherCompatible, left) && CollectionHelper.Contains(_otherCompatible, right))
                return;
            if (CollectionHelper.Contains(_overflowIgnorant, left) && left == right)
                return;
            throw new ArgumentException(string.Format("Adding {0} and {1} is not supported.", left, right));
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_left);
            ctx.Compile(_right);
            ctx.MarkSequencePointFor(expressionId);

            if (!_overflowCheck || CollectionHelper.Contains(_overflowIgnorant, _left.ExpressionType))
                ctx.Generator.Emit(OpCodes.Add);
            else if (CollectionHelper.Contains(_overflowUnsigned, _left.ExpressionType) && CollectionHelper.Contains(_overflowUnsigned, _right.ExpressionType))
                ctx.Generator.Emit(OpCodes.Add_Ovf_Un);
            else
                ctx.Generator.Emit(OpCodes.Add_Ovf);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            symbolGenerator
                .Write(_left)
                .Write(" + ")
                .Write(_right);
            return start.BlockTo(symbolGenerator.GetCurrentPosition());
        }
    }
}