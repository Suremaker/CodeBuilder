using System;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class FieldWriteExpression : VoidExpression
    {
        private readonly Expression _instance;
        private readonly FieldInfo _fieldInfo;
        private readonly Expression _value;

        public FieldWriteExpression(Expression instance, FieldInfo fieldInfo, Expression value)
        {
            Validators.NullCheck(fieldInfo, "fieldInfo");
            Validators.NullCheck(value, "value");
            if (!fieldInfo.IsStatic)
            {
                Validators.NullCheck(instance, "instance");
                Validators.HierarchyCheck(instance.ExpressionType, fieldInfo.DeclaringType, "Instance expression of type {0} does not match to type: {1}", "instance");
            }
            else if (instance != null)
                throw new ArgumentException("Static field cannot be written with instance parameter", "instance");
            Validators.HierarchyCheck(value.ExpressionType, fieldInfo.FieldType, "Value expression of type {0} does not match to type: {1}", "value");

            _instance = (instance == null) ? null : instance.EnsureCallableForm();
            _fieldInfo = fieldInfo;
            _value = value;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            if (_instance != null)
                ctx.Compile(_instance);
            ctx.Compile(_value);
            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit((_instance != null) ? OpCodes.Stfld : OpCodes.Stsfld, _fieldInfo);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            if (_instance != null)
                symbolGenerator.Write(_instance);
            var start = symbolGenerator.GetCurrentPosition();
            var end = symbolGenerator
                .Write(string.Format("{0}.{1} = ", (_instance == null) ? _fieldInfo.DeclaringType.FullName : string.Empty, _fieldInfo.Name))
                .Write(_value)
                .Write(";")
                .GetCurrentPosition();
            return start.BlockTo(end);
        }
    }
}