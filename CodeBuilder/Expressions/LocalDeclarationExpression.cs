using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class LocalDeclarationExpression : VoidExpression
    {
        private readonly LocalVariable _variable;
        private readonly Expression _initialValue;

        public LocalDeclarationExpression(LocalVariable variable)
        {
            Validators.NullCheck(variable, "variable");
            _variable = variable;
        }

        public LocalDeclarationExpression(LocalVariable variable, Expression initialValue)
            : this(variable)
        {
            Validators.NullCheck(initialValue, "initialValue");
            Validators.HierarchyCheck(initialValue.ExpressionType, variable.VariableType, "Unable to assign {0} to variable of type {1}", "initialValue");
            _initialValue = initialValue;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            var local = ctx.DeclareLocal(_variable);

            if (_initialValue != null)
            {
                ctx.Compile(_initialValue);

                ctx.MarkSequencePointFor(expressionId);
                LocalWriteExpression.EmitWriteLocal(ctx, local);
            }
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            symbolGenerator.Write(string.Format("{0} {1}", _variable.VariableType.FullName, _variable.Name));
            if (_initialValue != null)
                symbolGenerator.Write(" = ").Write(_initialValue);
            return start.BlockTo(symbolGenerator.WriteStatementEnd(";"));
        }
    }
}