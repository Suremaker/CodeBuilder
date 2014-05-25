using System.Reflection.Emit;
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

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.MarkSequencePointFor(expressionId);
            var local = ctx.GetLocal(_variable);
            if (_loadAddress)
                EmitLocalAddress(ctx, local);
            else
                EmitLocalValue(ctx, local);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start=symbolGenerator.GetCurrentPosition();
            return start.BlockTo(symbolGenerator.Write(_variable.Name).GetCurrentPosition());
        }

        protected override Expression ReturnCallableForm()
        {
            if (!ExpressionType.IsValueType || _loadAddress)
                return this;
            return new LocalReadExpression(_variable, true);
        }
    }
}