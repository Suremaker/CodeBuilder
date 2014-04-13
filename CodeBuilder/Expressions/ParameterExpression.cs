using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ParameterExpression : Expression
    {
        private readonly ushort _parameterId;

        public ParameterExpression(ushort parameterId, Type type)
            : base(type)
        {
            _parameterId = parameterId;
        }

        internal override void Compile(IBuildContext ctx)
        {
            ValidateParameter(ctx);
            if (_parameterId == 0) ctx.Generator.Emit(OpCodes.Ldarg_0);
            else if (_parameterId == 1) ctx.Generator.Emit(OpCodes.Ldarg_1);
            else if (_parameterId == 2) ctx.Generator.Emit(OpCodes.Ldarg_2);
            else if (_parameterId == 3) ctx.Generator.Emit(OpCodes.Ldarg_3);
            else if (_parameterId < 256) ctx.Generator.Emit(OpCodes.Ldarg_S, (byte)_parameterId);
            else ctx.Generator.Emit(OpCodes.Ldarg, _parameterId);
        }

        private void ValidateParameter(IBuildContext ctx)
        {
            if (ctx.Parameters.Length <= _parameterId)
                throw new InvalidOperationException(string.Format("Parameter index {0} is outside of bounds. Expected parameters: [{1}]", _parameterId, StringFormat.Join(ctx.Parameters, ",")));
            if (!ctx.Parameters[_parameterId].IsAssignableFrom(ExpressionType))
                throw new InvalidOperationException(string.Format("Parameter index {0} is of {1} type, while type {2} is expected", _parameterId, ExpressionType, ctx.Parameters[_parameterId]));
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendFormat(".arg [{0}] {1}", ExpressionType, _parameterId);
        }
    }
}