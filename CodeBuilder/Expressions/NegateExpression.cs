using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class NegateExpression : Expression
    {
        private static readonly Type[] _intTypes = new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(char) };
        private static readonly Type[] _preserveTypes = new[] { typeof(long), typeof(double), typeof(float) };
        private readonly Expression _value;

        public NegateExpression(Expression value)
            : base(DetermineResultType(Validators.NullCheck(value, "value")))
        {
            _value = value;
        }

        private static Type DetermineResultType(Expression value)
        {
            Validators.NumericPrimitiveCheck(value.ExpressionType, "value");

            foreach (var type in _intTypes)
                if (type == value.ExpressionType)
                    return typeof(int);

            foreach (var type in _preserveTypes)
                if (type == value.ExpressionType)
                    return value.ExpressionType;

            throw new ArgumentException(string.Format("Unsupported operation for type {0}. Please try to cast them first to type allowing negation without overflow.", value.ExpressionType), "value");
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_value);
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Neg);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.Write("-").Write(_value).GetCurrentPosition());
        }
    }
}