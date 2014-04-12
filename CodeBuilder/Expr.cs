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

        /// <summary>
        /// IfThen expression is always Void type.
        /// It will use Pop if then expression is not Void.
        /// </summary>
        /// <param name="predicate">If non-zero or not-null, then expression would be executed</param>
        /// <param name="thenExpression"></param>
        /// <returns></returns>
        public static IfThenExpression IfThen(Expression predicate,Expression thenExpression) { return new IfThenExpression(predicate,thenExpression);}
        /// <summary>
        /// IfThenElse expression is of thenExpression type.
        /// ThenExpression and elseExpression have to be of the same type.
        /// It is an equivalent of ?: operator
        /// </summary>
        /// <param name="predicate">If non-zero or not-null, then expression would be executed, otherwise elseExpression would be executed.</param>
        /// <param name="thenExpression"></param>
        /// <param name="elseExpression"></param>
        /// <returns></returns>
        public static IfThenElseExpression IfThenElse(Expression predicate,Expression thenExpression,Expression elseExpression) { return new IfThenElseExpression(predicate,thenExpression,elseExpression);}
    }
}