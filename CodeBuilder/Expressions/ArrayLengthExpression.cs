using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class ArrayLengthExpression : Expression
    {
        private readonly Expression _arrayInstance;

        internal ArrayLengthExpression(Expression arrayInstance)
            : base(typeof(int))
        {
            Validators.ArrayCheck(arrayInstance, "arrayInstance");
            _arrayInstance = arrayInstance;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ctx.Compile(_arrayInstance);
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Ldlen);//native int
            ctx.Generator.Emit(OpCodes.Conv_I4);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append(".length ");
            return _arrayInstance.Dump(builder);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var begin = symbolGenerator
                .Write(_arrayInstance)
                .GetCurrentPosition();

            return begin.BlockTo(symbolGenerator.Write(".Length()").GetCurrentPosition());
        }
    }
}