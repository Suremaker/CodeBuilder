﻿using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ParameterExpression : Expression
    {
        private readonly ushort _parameterId;
        private readonly bool _loadAddress;

        public ParameterExpression(ushort parameterId, Type type)
            : this(parameterId, type, false)
        {
        }

        private ParameterExpression(ushort parameterId, Type type, bool loadAddress)
            : base(Validators.NullCheck(type, "type"))
        {
            _parameterId = parameterId;
            _loadAddress = loadAddress;
        }

        private void EmitArgumentValue(IBuildContext ctx)
        {
            if (_parameterId == 0) ctx.Generator.Emit(OpCodes.Ldarg_0);
            else if (_parameterId == 1) ctx.Generator.Emit(OpCodes.Ldarg_1);
            else if (_parameterId == 2) ctx.Generator.Emit(OpCodes.Ldarg_2);
            else if (_parameterId == 3) ctx.Generator.Emit(OpCodes.Ldarg_3);
            else if (_parameterId < 256) ctx.Generator.Emit(OpCodes.Ldarg_S, (byte)_parameterId);
            else ctx.Generator.Emit(OpCodes.Ldarg, _parameterId);
        }

        private void EmitArgumentAddress(IBuildContext ctx)
        {
            if (_parameterId < 256) ctx.Generator.Emit(OpCodes.Ldarga_S, (byte)_parameterId);
            else ctx.Generator.Emit(OpCodes.Ldarga, _parameterId);
        }

        private void ValidateParameter(IBuildContext ctx)
        {
            if (ctx.Parameters.Length <= _parameterId)
                throw new InvalidOperationException(string.Format("Parameter index {0} is outside of bounds. Expected parameters: [{1}]", _parameterId, StringFormat.Join(ctx.Parameters, ",")));
            if (!Validators.IsInHierarchy(ctx.Parameters[_parameterId], ExpressionType))
                throw new InvalidOperationException(string.Format("Parameter index {0} is of {1} type, while type {2} is expected", _parameterId, ExpressionType, ctx.Parameters[_parameterId]));
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ValidateParameter(ctx);
            ctx.MarkSequencePointFor(expressionId);
            if (_loadAddress)
                EmitArgumentAddress(ctx);
            else
                EmitArgumentValue(ctx);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.Write(string.Format("p{0}", _parameterId)).GetCurrentPosition());
        }

        protected override Expression ReturnCallableForm()
        {
            if (!ExpressionType.IsValueType || _loadAddress)
                return this;
            return new ParameterExpression(_parameterId, ExpressionType, true);
        }
    }
}