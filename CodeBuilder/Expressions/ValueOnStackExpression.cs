using System;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    internal class ValueOnStackExpression : Expression
    {
        internal ValueOnStackExpression(Type type)
            : base(Validators.NullCheck(type, "type"))
        {
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            //value is already on stack
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            return builder;
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.GetCurrentPosition());
        }
    }
}