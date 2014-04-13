using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class FieldReadExpression : Expression
    {
        private readonly Expression _instance;
        private readonly FieldInfo _fieldInfo;

        public FieldReadExpression(Expression instance, FieldInfo fieldInfo)
            : base(fieldInfo.FieldType)
        {
            Validators.NullCheck(fieldInfo, "fieldInfo");
            if (!fieldInfo.IsStatic)
            {
                Validators.NullCheck(instance, "instance");
                Validators.HierarchyCheck(instance.ExpressionType, fieldInfo.DeclaringType, "Instance expression of type {0} does not match to type: {1}", "instance");
            }

            _instance = instance;
            _fieldInfo = fieldInfo;
        }

        internal override void Compile(IBuildContext ctx)
        {
            if (_instance != null)
                _instance.Compile(ctx);
            ctx.Generator.Emit((_instance != null) ? OpCodes.Ldfld : OpCodes.Ldsfld, _fieldInfo);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            if (_instance != null)
                _instance.Dump(builder);
            return builder.AppendFormat(".getField [{0}.{1}]", _fieldInfo.DeclaringType,_fieldInfo.Name);
        }
    }
}