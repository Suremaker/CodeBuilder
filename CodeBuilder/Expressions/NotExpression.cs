using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class NotExpression : Expression
    {
        private static readonly Type[] _intTypes = new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(char) };
        private static readonly Type[] _preservedTypes = new[] { typeof(int), typeof(uint), typeof(long), typeof(ulong) };
        private readonly Expression _value;

        public NotExpression(Expression value)
            : base(DetermineResultType(Validators.NullCheck(value, "value")))
        {
            _value = value;
        }

        private static Type DetermineResultType(Expression value)
        {
            foreach (var type in _intTypes)
                if (type == value.ExpressionType)
                    return typeof(int);

            foreach (var type in _preservedTypes)
                if (type == value.ExpressionType)
                    return type;

            throw new ArgumentException(string.Format("Expected integral type, got: {0}", value.ExpressionType), "value");
        }

        internal override void Compile(IBuildContext ctx)
        {
            _value.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Not);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append("~");
            return _value.Dump(builder);
        }
    }
}