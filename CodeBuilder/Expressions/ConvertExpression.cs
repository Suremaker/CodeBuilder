using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ConvertExpression : Expression
    {
        private class PCode
        {
            public int Size { get; private set; }
            public OpCode Code { get; private set; }
            public OpCode OvfCode { get; private set; }
            public OpCode CodeIfBigger { get; private set; }

            public PCode(int size, OpCode code, OpCode ovfCode) : this(size, code, ovfCode, code) { }
            public PCode(int size, OpCode code, OpCode ovfCode, OpCode codeIfBigger)
            {
                Size = size;
                Code = code;
                OvfCode = ovfCode;
                CodeIfBigger = codeIfBigger;
            }

            public OpCode ConversionFrom(PCode from, bool overflowCheck)
            {
                return overflowCheck ? OvfCode : (from.Size < Size ? CodeIfBigger : Code);
            }
        }

        private static readonly IDictionary<Type, PCode> _primitiveCodes = new Dictionary<Type, PCode>
        {
            {typeof (byte), new PCode(1, OpCodes.Conv_U1, OpCodes.Conv_Ovf_U1)},
            {typeof (sbyte), new PCode(1, OpCodes.Conv_I1, OpCodes.Conv_Ovf_I1)},
            {typeof (ushort), new PCode(2, OpCodes.Conv_U2, OpCodes.Conv_Ovf_U2)},
            {typeof (char), new PCode(2, OpCodes.Conv_U2, OpCodes.Conv_Ovf_U2)},
            {typeof (short), new PCode(2, OpCodes.Conv_I2, OpCodes.Conv_Ovf_I2)},
            {typeof (uint), new PCode(4, OpCodes.Conv_U4, OpCodes.Conv_Ovf_U4)},
            {typeof (int), new PCode(4, OpCodes.Conv_I4, OpCodes.Conv_Ovf_I4)},
            {typeof (ulong), new PCode(8, OpCodes.Conv_U8, OpCodes.Conv_Ovf_U8,OpCodes.Conv_I8)},
            {typeof (long), new PCode(8, OpCodes.Conv_I8, OpCodes.Conv_Ovf_I8)},

            {typeof (float), new PCode(4, OpCodes.Conv_R4, OpCodes.Conv_R4)},
            {typeof (double), new PCode(8, OpCodes.Conv_R8, OpCodes.Conv_R8)}
        };
        private readonly Expression _expression;
        private readonly bool _overflowCheck;
        private readonly Action<IBuildContext> _conversionMethod;

        public ConvertExpression(Expression expression, Type type, bool overflowCheck = false)
            : base(Validators.NullCheck(type, "type"))
        {
            Validators.NullCheck(expression, "expression");
            Validators.ConversionCheck(expression.ExpressionType, type, "Cannot convert type from {0} to {1}", "expression");
            _expression = expression;
            _overflowCheck = overflowCheck;
            _conversionMethod = DetermineOpCode(_expression.ExpressionType, ExpressionType);
        }

        private Action<IBuildContext> DetermineOpCode(Type castFrom, Type castTo)
        {
            if (castFrom == castTo)
                return EmitNothing;

            if (castFrom.IsValueType && !castTo.IsValueType)
                return EmitBox;
            if (castTo.IsValueType && !castFrom.IsValueType)
                return EmitUnbox;

            if (castTo.IsPrimitive && castFrom.IsPrimitive)
                return EmitConv;

            if (!castTo.IsAssignableFrom(castFrom))
                return EmitCast;

            //else no action is needed
            return EmitNothing;
        }

        private static OpCode FindPrimitiveConvCode(Type castFrom, Type castTo, bool overflowCheck)
        {
            if ((castTo == typeof(double) || castTo == typeof(float)) &&
                (castFrom == typeof(uint) || castFrom == typeof(ulong)))
                return OpCodes.Conv_R_Un;
            var from = _primitiveCodes[castFrom];
            var to = _primitiveCodes[castTo];
            return to.ConversionFrom(from, overflowCheck);
        }

        internal override void Compile(IBuildContext ctx)
        {
            _expression.Compile(ctx);
            _conversionMethod(ctx);
        }

        internal void EmitBox(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Box, _expression.ExpressionType);
        }

        internal void EmitUnbox(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Unbox_Any, ExpressionType);
        }

        internal void EmitCast(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Castclass, ExpressionType);
        }

        internal void EmitConv(IBuildContext ctx)
        {
            var code = FindPrimitiveConvCode(_expression.ExpressionType, ExpressionType, _overflowCheck);
            if (!Equals(code, OpCodes.Nop))
                ctx.Generator.Emit(code);
            if (Equals(code, OpCodes.Conv_R_Un))
                ctx.Generator.Emit(ExpressionType == typeof(double) ? OpCodes.Conv_R8 : OpCodes.Conv_R4);
        }

        internal void EmitNothing(IBuildContext ctx)
        {
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendFormat("({0}) ", ExpressionType);
            return _expression.Dump(builder);
        }
    }
}