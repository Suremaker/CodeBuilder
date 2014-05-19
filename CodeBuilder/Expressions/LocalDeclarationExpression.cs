using System.Text;
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

        internal override void Compile(IBuildContext ctx)
        {
            var local = ctx.DeclareLocal(_variable);

            if (_initialValue != null)
                LocalWriteExpression.Compile(ctx, local, _initialValue);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendFormat(".local {0}", _variable);
            if (_initialValue != null)
            {
                builder.Append(" = ");
                _initialValue.Dump(builder);
            }
            return builder.AppendLine(";");
        }
    }
}