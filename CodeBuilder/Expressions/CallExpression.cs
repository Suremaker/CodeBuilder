using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class CallExpression : Expression
    {
        private readonly Expression _instance;
        private readonly MethodInfo _methodInfo;
        private readonly Expression[] _arguments;

        public CallExpression(Expression instance, MethodInfo methodInfo, params Expression[] arguments)
            : base(methodInfo.ReturnType)
        {
            Validators.NullCheck(methodInfo, "methodInfo");
            Validators.NullCheck(arguments, "arguments");
            if (!methodInfo.IsStatic)
                Validators.NullCheck(instance, "instance");
            if (instance != null)
                Validators.AssignableCheck(methodInfo.DeclaringType, instance.ExpressionType, "Instance expression of type {0} does not match to type: {1}", "instance");

            Validators.ParameterCheck(methodInfo, arguments, "arguments");

            _instance = instance;
            _methodInfo = methodInfo;
            _arguments = arguments;
        }

        internal override void Compile(IBuildContext ctx)
        {
            if (_instance != null)
                _instance.Compile(ctx);
            foreach (var value in _arguments)
                value.Compile(ctx);
            ctx.Generator.Emit(OpCodes.Call, _methodInfo);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            if (_instance != null)
                _instance.Dump(builder);
            builder.AppendFormat(".call [{0}] (", _methodInfo);
            for (int i = 0; i < _arguments.Length; ++i)
            {
                _arguments[i].Dump(builder);
                if (i + 1 < _arguments.Length) builder.Append(", ");
            }
            return builder.Append(")");
        }
    }
}