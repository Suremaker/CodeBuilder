using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ConstExpression : Expression
    {
        private delegate void CompileFn(IBuildContext ctx);
        private readonly object _value;
        private readonly CompileFn _action;

        public ConstExpression(int value)
            : base(typeof(int))
        {
            _value = value;
            _action = CompileInt;
        }

        public ConstExpression(long value)
            : base(typeof(long))
        {
            _value = value;
            _action = CompileLong;
        }

        public ConstExpression(float value)
            : base(typeof(float))
        {
            _value = value;
            _action = CompileFloat;
        }

        public ConstExpression(double value)
            : base(typeof(double))
        {
            _value = value;
            _action = CompileDouble;
        }

        public ConstExpression(string value)
            : base(typeof(string))
        {
            _value = value;
            _action = CompileStr;
        }

        public ConstExpression(Type value)
            : base(typeof(Type))
        {
            Validators.NullCheck(value, "value");
            _value = value;
            _action = CompileType;
        }

        public ConstExpression(byte value)
            : base(typeof(byte))
        {
            _value = (int)value;
            _action = CompileInt;
        }

        public ConstExpression(bool value)
            : base(typeof(bool))
        {
            _value = value ? 1 : 0;
            _action = CompileInt;
        }

        private void CompileStr(IBuildContext ctx)
        {
            var value = (string)_value;
            if (value != null)
                ctx.Generator.Emit(OpCodes.Ldstr, value);
            else
                ctx.Generator.Emit(OpCodes.Ldnull);
        }

        private void CompileInt(IBuildContext ctx)
        {
            EmitIntCode(ctx, (int)_value);
        }

        private void CompileType(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Ldtoken, (Type)_value);
            ctx.Generator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
        }

        private void CompileLong(IBuildContext ctx)
        {
            var value = (long)_value;
            if (value >= int.MinValue && value <= int.MaxValue)
            {
                EmitIntCode(ctx, (int)value);
                ctx.Generator.Emit(OpCodes.Conv_I8);
            }
            else
                ctx.Generator.Emit(OpCodes.Ldc_I8, value);
        }

        private void CompileFloat(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Ldc_R4, (float)_value);
        }

        private void CompileDouble(IBuildContext ctx)
        {
            ctx.Generator.Emit(OpCodes.Ldc_R8, (double)_value);
        }

        private static void EmitIntCode(IBuildContext ctx, int value)
        {
            var opCode = FindIntOpCode(value);
            if (opCode == OpCodes.Ldc_I4_S)
                ctx.Generator.Emit(opCode, (sbyte)value);
            else if (opCode == OpCodes.Ldc_I4)
                ctx.Generator.Emit(opCode, value);
            else
                ctx.Generator.Emit(opCode);
        }

        private static OpCode FindIntOpCode(int value)
        {
            if (value == 0) return OpCodes.Ldc_I4_0;
            if (value == 1) return OpCodes.Ldc_I4_1;
            if (value == 2) return OpCodes.Ldc_I4_2;
            if (value == 3) return OpCodes.Ldc_I4_3;
            if (value == 4) return OpCodes.Ldc_I4_4;
            if (value == 5) return OpCodes.Ldc_I4_5;
            if (value == 6) return OpCodes.Ldc_I4_6;
            if (value == 7) return OpCodes.Ldc_I4_7;
            if (value == 8) return OpCodes.Ldc_I4_8;
            if (value == -1) return OpCodes.Ldc_I4_M1;
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) return OpCodes.Ldc_I4_S;
            return OpCodes.Ldc_I4;
        }

        internal override void Compile(IBuildContext ctx)
        {
            _action(ctx);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendFormat(".const [{0}] {1}", ExpressionType, _value);
        }
    }
}