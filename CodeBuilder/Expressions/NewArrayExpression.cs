using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

namespace CodeBuilder.Expressions
{
    public class NewArrayExpression : Expression
    {
        private readonly Type _elementType;
        private readonly Expression _count;

        internal NewArrayExpression(Type elementType, Expression count)
            : base(Validators.NullCheck(elementType, "elementType").MakeArrayType())
        {
            Validators.NullCheck(count, "count");
            Validators.IntegralCheck(count.ExpressionType, "Array construction requires parameter of integral type, got: {0}", "count");
            _elementType = elementType;
            _count = count;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_count);
            EmitHelper.ConvertToNativeInt(ctx, _count.ExpressionType);
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Newarr, _elementType);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            return symbolGenerator.GetCurrentPosition().BlockTo(
                symbolGenerator.Write(string.Format("new {0} [", _elementType.FullName)).Write(_count).Write("]").GetCurrentPosition());
        }
    }
}