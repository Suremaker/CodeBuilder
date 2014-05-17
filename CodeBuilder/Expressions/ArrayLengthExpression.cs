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

        internal override void Compile(IBuildContext ctx)
        {
            _arrayInstance.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Ldlen);//native int
            ctx.Generator.Emit(OpCodes.Conv_I4);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append(".length ");
            return _arrayInstance.Dump(builder);
        }
    }
}