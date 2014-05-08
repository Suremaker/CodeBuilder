using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class NewExpression : Expression
    {
        private readonly ConstructorInfo _constructorInfo;
        private readonly Expression[] _arguments;

        public NewExpression(Type type, Expression[] arguments)
            : base(type)
        {
            Validators.NullCollectionElementsCheck(arguments, "arguments");

            var types = new Type[arguments.Length];
            for (int i = 0; i < arguments.Length; ++i)
                types[i] = arguments[i].ExpressionType;

            var constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
            if (constructorInfo == null && (!type.IsValueType || arguments.Length > 0))
                throw new ArgumentException(FormatMissingConstructorException(type, types), "arguments");
            _constructorInfo = constructorInfo;
            _arguments = arguments;
        }

        private static string FormatMissingConstructorException(Type type, Type[] types)
        {
            return string.Format("No matching constructor found for type {0} with parameters: [{1}]", type, StringFormat.Join(types, ", "));
        }

        internal override void Compile(IBuildContext ctx)
        {
            foreach (var argument in _arguments)
                argument.Compile(ctx);
            if (_constructorInfo == null)
                EmitStructInit(ctx);
            else
                ctx.Generator.Emit(OpCodes.Newobj, _constructorInfo);
        }

        private void EmitStructInit(IBuildContext ctx)
        {
            var local = ctx.Generator.DeclareLocal(ExpressionType);
            ctx.Generator.Emit(OpCodes.Ldloca, local);
            ctx.Generator.Emit(OpCodes.Initobj, ExpressionType);
            ctx.Generator.Emit(OpCodes.Ldloc, local);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.Append(".new [").Append(ExpressionType).Append("] (");
            for (int i = 0; i < _arguments.Length; i++)
            {
                _arguments[i].Dump(builder);
                if (i + 1 < _arguments.Length) builder.Append(", ");
            }
            return builder.Append(")");
        }
    }
}