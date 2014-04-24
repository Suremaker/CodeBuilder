using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class LocalReadExpression : Expression
    {
        private readonly LocalVariable _variable;
        private readonly bool _loadAddress;

        public LocalReadExpression(LocalVariable variable) : this(variable, false) { }
        private LocalReadExpression(LocalVariable variable, bool loadAddress)
            : base(Validators.NullCheck(variable, "variable").VariableType)
        {
            _variable = variable;
            _loadAddress = loadAddress;
        }

        internal override void Compile(IBuildContext ctx)
        {
            var local = ctx.GetLocalIndex(_variable);
            if (_loadAddress)
                EmitLocalAddress(ctx, local);
            else
                EmitLocalValue(ctx, local);
        }

        private static void EmitLocalValue(IBuildContext ctx, LocalBuilder local)
        {
            if (local.LocalIndex == 0) ctx.Generator.Emit(OpCodes.Ldloc_0);
            else if (local.LocalIndex == 1) ctx.Generator.Emit(OpCodes.Ldloc_1);
            else if (local.LocalIndex == 2) ctx.Generator.Emit(OpCodes.Ldloc_2);
            else if (local.LocalIndex == 3) ctx.Generator.Emit(OpCodes.Ldloc_3);
            else if (local.LocalIndex < 256) ctx.Generator.Emit(OpCodes.Ldloc_S, (byte)local.LocalIndex);
            else ctx.Generator.Emit(OpCodes.Ldloc, local);
        }

        private static void EmitLocalAddress(IBuildContext ctx, LocalBuilder local)
        {
            if (local.LocalIndex < 256)
                ctx.Generator.Emit(OpCodes.Ldloca_S, (byte)local.LocalIndex);
            else
                ctx.Generator.Emit(OpCodes.Ldloca, local);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendFormat(".getlocal [{0}]", _variable);
        }

        protected override Expression ReturnCallableForm()
        {
            if (!ExpressionType.IsValueType || _loadAddress)
                return this;
            return new LocalReadExpression(_variable, true);
        }
    }
}