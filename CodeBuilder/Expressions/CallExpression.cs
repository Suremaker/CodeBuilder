using System;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

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
                Validators.HierarchyCheck(instance.ExpressionType, methodInfo.DeclaringType, "Instance expression of type {0} does not match to type: {1}", "instance");

            Validators.ParameterCheck(methodInfo, arguments, "arguments");

            _instance = (instance == null) ? null : instance.EnsureCallableForm();
            _methodInfo = methodInfo;
            _arguments = arguments;
            _forceNonVirtualCall = forceNonVirtualCall;
        }

        private string GetMethodCodeHeader()
        {
            return string.Format("{0}.{1}(", (_instance == null) ? _methodInfo.DeclaringType.FullName : "", _methodInfo.Name);
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            var forceNonVirtualCall = _forceNonVirtualCall;
            if (_instance != null)
            {
                ctx.Compile(_instance);
                if (_instance.ExpressionType.IsValueType)
                    forceNonVirtualCall = true;
            }

            foreach (var argument in _arguments)
                ctx.Compile(argument);

            ctx.MarkSequencePointFor(expressionId);

            if (!forceNonVirtualCall && _methodInfo.IsVirtual && !_methodInfo.IsFinal)
                ctx.Generator.Emit(OpCodes.Callvirt, _methodInfo.GetBaseDefinition());
            else
                ctx.Generator.Emit(OpCodes.Call, _methodInfo);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var begin = symbolGenerator.GetCurrentPosition();
            if (_instance != null)
                symbolGenerator.Write(_instance);
            symbolGenerator.Write(GetMethodCodeHeader());
            for (int i = 0; i < _arguments.Length; ++i)
            {
                symbolGenerator.Write(_arguments[i]);
                if (i + 1 < _arguments.Length)
                    symbolGenerator.Write(", ");
            }
            symbolGenerator.Write(")");
            if (ExpressionType == typeof(void))
                return begin.BlockTo(symbolGenerator.Write(";").GetCurrentPosition());
            return begin.BlockTo(symbolGenerator.GetCurrentPosition());
        }
    }
}