using System;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Expressions;

namespace CodeBuilder
{
    public static class Expr
    {
        public static ConstExpression Constant(string value) { return new ConstExpression(value); }
        public static ConstExpression Constant(int value) { return new ConstExpression(value); }

        public static FieldWriteExpression FieldWrite(FieldInfo fieldInfo, Expression value) { return new FieldWriteExpression(null, fieldInfo, value); }
        public static FieldWriteExpression FieldWrite(Expression instance, FieldInfo fieldInfo, Expression value) { return new FieldWriteExpression(instance, fieldInfo, value); }
        public static ReturnExpression Return() { return new ReturnExpression(); }
        public static ReturnExpression Return(Expression value) { return new ReturnExpression(value); }

        public static CallExpression Call(Expression instance, MethodInfo methodInfo, params Expression[] arguments) { return new CallExpression(instance, methodInfo, arguments); }
        public static CallExpression Call(MethodInfo methodInfo, params Expression[] arguments) { return Call(null, methodInfo, arguments); }

        public static PopExpression Pop(Expression expression) { return new PopExpression(expression); }

        public static NewExpression New(Type type, params Expression[] arguments) { return new NewExpression(type, arguments); }

        public static FieldReadExpression ReadField(FieldBuilder fieldInfo) { return new FieldReadExpression(null, fieldInfo); }
        public static FieldReadExpression ReadField(Expression instance, FieldBuilder fieldInfo) { return new FieldReadExpression(instance, fieldInfo); }

        public static ParameterExpression Parameter(ushort parameterId, Type type) { return new ParameterExpression(parameterId, type); }

        public static ConvertExpression Convert(Expression expression, Type type) { return new ConvertExpression(expression, type); }
    }
}