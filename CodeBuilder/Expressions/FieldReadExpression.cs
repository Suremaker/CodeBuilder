using System;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class FieldReadExpression : Expression
    {
        private readonly Expression _instance;
        private readonly FieldInfo _fieldInfo;
        private readonly bool _loadAddress;

        public FieldReadExpression(Expression instance, FieldInfo fieldInfo) : this(instance, fieldInfo, false) { }

        private FieldReadExpression(Expression instance, FieldInfo fieldInfo, bool loadAddress)
            : base(Validators.NullCheck(fieldInfo, "fieldInfo").FieldType)
        {
            Validators.NullCheck(fieldInfo, "fieldInfo");
            if (!fieldInfo.IsStatic)
            {
                Validators.NullCheck(instance, "instance");
                Validators.HierarchyCheck(instance.ExpressionType, fieldInfo.DeclaringType, "Instance expression of type {0} does not match to type: {1}", "instance");
            }
            else if (instance != null)
                throw new ArgumentException("Static field cannot be read with instance parameter", "instance");
            _instance = instance;
            _fieldInfo = fieldInfo;
            _loadAddress = loadAddress;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            if (_instance != null)
                ctx.Compile(_instance);

            ctx.MarkSequencePointFor(expressionId);

            if (_loadAddress)
                ctx.Generator.Emit((_instance != null) ? OpCodes.Ldflda : OpCodes.Ldsflda, _fieldInfo);
            else
                ctx.Generator.Emit((_instance != null) ? OpCodes.Ldfld : OpCodes.Ldsfld, _fieldInfo);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            if (_instance != null)
                symbolGenerator.Write(_instance);
            var start = symbolGenerator.GetCurrentPosition();
            var end = symbolGenerator
                .Write(string.Format("{0}.{1}",
                    (_instance == null) ? _fieldInfo.DeclaringType.FullName : string.Empty,
                    _fieldInfo.Name))
                .GetCurrentPosition();
            return start.BlockTo(end);
        }

        protected override Expression ReturnCallableForm()
        {
            if (!ExpressionType.IsValueType || _loadAddress)
                return this;
            return new FieldReadExpression(_instance, _fieldInfo, true);
        }
    }
}