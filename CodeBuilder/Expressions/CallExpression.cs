using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class CallExpression : Expression
    {
        private readonly Expression _instance;
        private readonly MethodInfo _methodInfo;
        private readonly Expression[] _arguments;
        private readonly bool _forceNonVirtualCall;

        internal CallExpression(Expression instance, MethodInfo methodInfo, Expression[] arguments, bool forceNonVirtualCall = false)
            : base(Validators.NullCheck(methodInfo, "methodInfo").ReturnType)
        {
            Validators.NullCheck(methodInfo, "methodInfo");
            Validators.NullCheck(arguments, "arguments");
            if (!methodInfo.IsStatic)
                Validators.NullCheck(instance, "instance");
            else if (instance != null)
                throw new ArgumentException("Static method cannot be called with instance parameter", "instance");
            if (instance != null)
            {
                Validators.HierarchyCheck(instance.ExpressionType, methodInfo.DeclaringType, "Instance expression of type {0} does not match to type: {1}", "instance");
                if (methodInfo.DeclaringType.IsValueType)
                    throw new NotSupportedException("Calling instance methods on struct types is not supported yet.");
            }

            Validators.ParameterCheck(methodInfo, arguments, "arguments");

            _instance = instance;
            _methodInfo = methodInfo;
            _arguments = arguments;
            _forceNonVirtualCall = forceNonVirtualCall;
        }

        internal override void Compile(IBuildContext ctx)
        {
            if (_instance != null)
                _instance.Compile(ctx);
            foreach (var value in _arguments)
                value.Compile(ctx);

            if (!_forceNonVirtualCall && _methodInfo.IsVirtual && !_methodInfo.IsFinal)
                ctx.Generator.Emit(OpCodes.Callvirt, _methodInfo);
            else
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