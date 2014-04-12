using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
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
            Validators.NullCheck(value, "value");
            if (!fieldInfo.IsStatic)
            {
                Validators.NullCheck(instance, "instance");
                Validators.HierarchyCheck(instance.ExpressionType, fieldInfo.DeclaringType, "Instance expression of type {0} does not match to type: {1}", "instance");
            }
            Validators.HierarchyCheck(value.ExpressionType, fieldInfo.FieldType, "Value expression of type {0} does not match to type: {1}", "value");

            _instance = instance;
            _fieldInfo = fieldInfo;
            _value = value;
        }

        internal override void Compile(IBuildContext ctx)
        {
            if (_instance != null)
                _instance.Compile(ctx);
            _value.Compile(ctx);
            ctx.Generator.Emit((_instance != null) ? OpCodes.Stfld : OpCodes.Stsfld, _fieldInfo);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            if (_instance != null)
                _instance.Dump(builder);
            builder.AppendFormat(".setField [{0}.{1}] = ", _fieldInfo.DeclaringType, _fieldInfo.Name);
            _value.Dump(builder);
            return builder.AppendLine(";");
        }
    }
}