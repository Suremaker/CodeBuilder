using System;
using System.Text;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    internal class ValueOnStackExpression : Expression
    {
        internal ValueOnStackExpression(Type type)
            : base(Validators.NullCheck(type, "type"))
        {
        }

        internal override void Compile(IBuildContext ctx)
        {
            //value is already on stack
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder.AppendFormat("$ [{0}]", ExpressionType);
        }
    }
}