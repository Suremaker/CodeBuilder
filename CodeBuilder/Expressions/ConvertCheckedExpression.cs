using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{

    public class ConvertCheckedExpression : ConvertExpression
    {
        private static readonly IDictionary<Type, PMap> _primitiveMappings = new Dictionary<Type, PMap>();

        private class PMap
        {
            public OpCode FromSignedCode { get; private set; }
            public OpCode FromUnsignedCode { get; private set; }
            public bool IsSigned { get; private set; }
            public PMap(OpCode fromSignedCode, OpCode fromUnsignedCode, bool isSigned)
            {
                IsSigned = isSigned;
                FromUnsignedCode = fromUnsignedCode;
                FromSignedCode = fromSignedCode;
            }

            public OpCode ConversionFrom(PMap from)
            {
                return from.IsSigned ? FromSignedCode : FromUnsignedCode;
            }
        }

        static ConvertCheckedExpression()
        {
            RegisterPrimitive(typeof(byte), OpCodes.Conv_Ovf_U1, OpCodes.Conv_Ovf_U1_Un, false);
            RegisterPrimitive(typeof(sbyte), OpCodes.Conv_Ovf_I1, OpCodes.Conv_Ovf_I1_Un, true);
            RegisterPrimitive(typeof(ushort), OpCodes.Conv_Ovf_U2, OpCodes.Conv_Ovf_U2_Un, false);
            RegisterPrimitive(typeof(char), OpCodes.Conv_Ovf_U2, OpCodes.Conv_Ovf_U2_Un, false);
            RegisterPrimitive(typeof(short), OpCodes.Conv_Ovf_I2, OpCodes.Conv_Ovf_I2_Un, true);
            RegisterPrimitive(typeof(uint), OpCodes.Conv_Ovf_U4, OpCodes.Conv_Ovf_U4_Un, false);
            RegisterPrimitive(typeof(int), OpCodes.Conv_Ovf_I4, OpCodes.Conv_Ovf_I4_Un, true);
            RegisterPrimitive(typeof(ulong), OpCodes.Conv_Ovf_U8, OpCodes.Conv_Ovf_U8_Un, false);
            RegisterPrimitive(typeof(long), OpCodes.Conv_Ovf_I8, OpCodes.Conv_Ovf_I8_Un, true);

            RegisterPrimitive(typeof(float), OpCodes.Conv_R4, OpCodes.Conv_R4, true);
            RegisterPrimitive(typeof(double), OpCodes.Conv_R8, OpCodes.Conv_R8, true);
        }

        private static void RegisterPrimitive(Type type, OpCode fromSignedCode, OpCode fromUnsignedCode, bool isSigned)
        {
            _primitiveMappings.Add(type, new PMap(fromSignedCode, fromUnsignedCode, isSigned));
        }


        public ConvertCheckedExpression(Expression expression, Type type)
            : base(expression, type)
        {
        }

        private static OpCode FindPrimitiveConvCode(Type castFrom, Type castTo)
        {
            var from = _primitiveMappings[castFrom];
            var to = _primitiveMappings[castTo];
            return to.ConversionFrom(from);
        }

        protected override void EmitConv(IBuildContext ctx)
        {
            var code = FindPrimitiveConvCode(Expression.ExpressionType, ExpressionType);
            if (!Equals(code, OpCodes.Nop))
                ctx.Generator.Emit(code);
        }
    }
}