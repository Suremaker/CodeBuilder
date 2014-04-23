using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    public class ConvertUncheckedExpression : ConvertExpression
    {
        private static readonly IDictionary<Type, PMap> _primitiveMappings = new Dictionary<Type, PMap>();
        private class PMap
        {
            public int Size { get; private set; }
            public Type Type { get; private set; }
            public OpCode Code { get; private set; }
            public bool IsSigned { get; private set; }
            public bool IsFloat { get; private set; }
            public PMap Other { get; set; }
            public PMap(int size, Type type, OpCode code, bool isSigned)
            {
                Size = size;
                Type = type;
                Code = code;
                IsSigned = isSigned;
                IsFloat = (Type == typeof(float) || Type == typeof(double));
            }

            public OpCode ConversionFrom(PMap from)
            {
                if (from.Size < Size && IsSigned != from.IsSigned && Other != null && !from.IsFloat)
                    return Other.Code;
                return Code;
            }
        }

        static ConvertUncheckedExpression()
        {
            RegisterPrimitive(1, typeof(byte), typeof(sbyte), OpCodes.Conv_U1, OpCodes.Conv_I1);
            RegisterPrimitive(2, typeof(ushort), typeof(short), OpCodes.Conv_U2, OpCodes.Conv_I2);
            RegisterPrimitive(4, typeof(uint), typeof(int), OpCodes.Conv_U4, OpCodes.Conv_I4);
            RegisterPrimitive(8, typeof(ulong), typeof(long), OpCodes.Conv_U8, OpCodes.Conv_I8);
            _primitiveMappings.Add(typeof(char), new PMap(2, typeof(char), OpCodes.Conv_U2, false));
            _primitiveMappings.Add(typeof(float), new PMap(4, typeof(float), OpCodes.Conv_R4, true));
            _primitiveMappings.Add(typeof(double), new PMap(8, typeof(double), OpCodes.Conv_R8, true));
        }

        private static void RegisterPrimitive(int size, Type unsignedType, Type signedType, OpCode unsignedCode, OpCode signedCode)
        {
            var umap = new PMap(size, unsignedType, unsignedCode, false);
            var smap = new PMap(size, signedType, signedCode, true);
            umap.Other = smap;
            smap.Other = umap;
            _primitiveMappings.Add(unsignedType, umap);
            _primitiveMappings.Add(signedType, smap);
        }

        public ConvertUncheckedExpression(Expression expression, Type type)
            : base(expression, type)
        {
        }

        private static OpCode FindPrimitiveConvCode(Type castFrom, Type castTo)
        {
            var from = _primitiveMappings[castFrom];
            var to = _primitiveMappings[castTo];
            return (to.IsFloat && !from.IsSigned && from.Size >= 4)
                ? OpCodes.Conv_R_Un
                : to.ConversionFrom(from);
        }

        protected override void EmitConv(IBuildContext ctx)
        {
            var code = FindPrimitiveConvCode(Expression.ExpressionType, ExpressionType);
            if (!Equals(code, OpCodes.Nop))
                ctx.Generator.Emit(code);
            if (Equals(code, OpCodes.Conv_R_Un))
                ctx.Generator.Emit(_primitiveMappings[ExpressionType].Code);
        }
    }
}