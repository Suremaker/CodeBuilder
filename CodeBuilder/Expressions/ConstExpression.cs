using System.Reflection.Emit;
using System.Text;

namespace CodeBuilder.Expressions
{
    public class ConstExpression : Expression
    {
        private delegate void CompileFn(IBuildContext ctx);
        private readonly object _value;
        private readonly OpCode _opCode;
        private readonly CompileFn _action;

        public ConstExpression(int value)
            : base(typeof(int))
        {
            _value = value;
            _opCode = FindIntOpCode(value);
            _action = CompileInt;
        }

        public ConstExpression(string value)
            : base(typeof(string))
        {
            _value = value;
            _opCode = OpCodes.Ldstr;
            _action = CompileStr;
        }

        private void CompileStr(IBuildContext ctx)
        {
            ctx.Generator.Emit(_opCode, (string)_value);
        }

        private void CompileInt(IBuildContext ctx)
        {
            ctx.Generator.Emit(_opCode, (int)_value);
        }

        private OpCode FindIntOpCode(int value)
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
            if (value >= -128 && value <= 127) return OpCodes.Ldc_I4_S;
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