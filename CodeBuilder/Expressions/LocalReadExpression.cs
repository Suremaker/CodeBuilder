using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class LocalReadExpression : Expression
    {
        private readonly LocalVariable _variable;

        public LocalReadExpression(LocalVariable variable)
            : base(Validators.NullCheck(variable, "variable").VariableType)
        {
            _variable = variable;
        }

        internal override void Compile(IBuildContext ctx)
        {
            var local = ctx.GetLocalIndex(_variable);
            if (local.LocalIndex == 0) ctx.Generator.Emit(OpCodes.Ldloc_0);
            else if (local.LocalIndex == 1) ctx.Generator.Emit(OpCodes.Ldloc_1);
            else if (local.LocalIndex == 2) ctx.Generator.Emit(OpCodes.Ldloc_2);
            else if (local.LocalIndex == 3) ctx.Generator.Emit(OpCodes.Ldloc_3);
            else if (local.LocalIndex < 256) ctx.Generator.Emit(OpCodes.Ldloc_S, local);
            else ctx.Generator.Emit(OpCodes.Ldloc, local);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendFormat(".getlocal [{0}]", _variable);
        }
    }
}