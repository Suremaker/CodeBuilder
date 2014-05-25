using System;
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
            if (!Validators.IsPrimitiveOrEnum(left) || !Validators.IsPrimitiveOrEnum(right))
                throw new ArgumentException(string.Format("Comparison of non primitive types is not supported: Left={0}, Right={1}", left, right));
            if (CollectionHelper.Contains(_compatible, left) && CollectionHelper.Contains(_compatible, right))
                return;
            if (CollectionHelper.Contains(_otherSupported, left) && left == right)
                return;
            if (left.IsEnum && right.IsEnum && right == left)
                return;
            throw new ArgumentException(string.Format("Comparison of {0} and {1} is not supported. Try to cast left or right value to corresponding type.", left, right));
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_left);
            ctx.Compile(_right);
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(GetOpCode(CollectionHelper.Contains(_unsigned, _left.ExpressionType)));
        }

        protected abstract OpCode GetOpCode(bool isUnsigned);


        internal override StringBuilder Dump(StringBuilder builder)
        {
            _left.Dump(builder);
            builder.Append(GetDumpSymbol());
            return _right.Dump(builder);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var begin = symbolGenerator.GetCurrentPosition();
            return begin.BlockTo(symbolGenerator.Write(_left).Write(GetDumpSymbol()).Write(_right).GetCurrentPosition());
        }

        protected abstract string GetDumpSymbol();
    }
}