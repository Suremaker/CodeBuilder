using System;
using System.Reflection;
using CodeBuilder.Expressions;
using CodeBuilder.Helpers;

namespace CodeBuilder
{
    public static class Expr
    {
        public static Expression Constant(bool value) { return new ConstExpression(value); }
        public static Expression Constant(byte value) { return new ConstExpression(value); }
        public static ConstExpression Constant(string value) { return new ConstExpression(value); }
        public static ConstExpression Constant(int value) { return new ConstExpression(value); }
        public static ConstExpression Constant(long value) { return new ConstExpression(value); }
        public static ConstExpression Constant(float value) { return new ConstExpression(value); }
        public static ConstExpression Constant(double value) { return new ConstExpression(value); }
        public static Expression Constant(Type value) { return new ConstExpression(value); }
        public static FieldWriteExpression WriteField(FieldInfo fieldInfo, Expression value) { return new FieldWriteExpression(null, fieldInfo, value); }
        public static FieldWriteExpression WriteField(Expression instance, FieldInfo fieldInfo, Expression value) { return new FieldWriteExpression(instance, fieldInfo, value); }
        public static ReturnExpression Return() { return new ReturnExpression(); }
        public static ReturnExpression Return(Expression value) { return new ReturnExpression(value); }

        /// <summary>
        /// Calls instance method specified by methodInfo in a way that even if it would be a virtual method, it would be called in non-virtual way.
        /// It can be used to implement base calls.
        /// </summary>
        public static CallExpression CallExact(Expression instance, MethodInfo methodInfo, params Expression[] arguments) { return new CallExpression(instance, methodInfo, arguments, true); }
        public static CallExpression Call(Expression instance, MethodInfo methodInfo, params Expression[] arguments) { return new CallExpression(instance, methodInfo, arguments); }
        public static CallExpression Call(MethodInfo methodInfo, params Expression[] arguments) { return new CallExpression(null, methodInfo, arguments); }

        public static CallExpression Call(Expression instance, string methodName, Type[] genericArguments, params Expression[] arguments) { return new CallExpression(instance, MemberHelper.FindMethod(instance, methodName, genericArguments, arguments), arguments); }
        public static CallExpression Call(Type methodOwner, string methodName, Type[] genericArguments, params Expression[] arguments) { return new CallExpression(null, MemberHelper.FindMethod(methodOwner, methodName, genericArguments, arguments), arguments); }
        public static CallExpression Call(Expression instance, string methodName, params Expression[] arguments) { return new CallExpression(instance, MemberHelper.FindMethod(instance, methodName, null, arguments), arguments); }
        public static CallExpression Call(Type methodOwner, string methodName, params Expression[] arguments) { return new CallExpression(null, MemberHelper.FindMethod(methodOwner, methodName, null, arguments), arguments); }

        public static PopExpression Pop(Expression expression) { return new PopExpression(expression); }

        public static NewExpression New(Type type, params Expression[] arguments) { return new NewExpression(type, arguments); }

        public static FieldReadExpression ReadField(FieldInfo fieldInfo) { return new FieldReadExpression(null, fieldInfo); }
        public static FieldReadExpression ReadField(Expression instance, FieldInfo fieldInfo) { return new FieldReadExpression(instance, fieldInfo); }
        public static FieldReadExpression ReadField(Expression instance, string fieldName) { return new FieldReadExpression(instance, MemberHelper.FindField(instance, fieldName)); }

        public static ParameterExpression Parameter(ushort parameterId, Type type) { return new ParameterExpression(parameterId, type); }

        public static ConvertExpression Convert(Expression expression, Type type) { return new ConvertUncheckedExpression(expression, type); }
        public static ConvertExpression ConvertChecked(Expression expression, Type type) { return new ConvertCheckedExpression(expression, type); }
        public static ThrowExpression Throw(Expression exceptionExpression) { return new ThrowExpression(exceptionExpression); }
        public static RethrowExpression Rethrow() { return new RethrowExpression(); }
        public static TryCatchFinallyExpression TryFinally(Expression tryExpression, Expression finallyExpression) { return new TryCatchFinallyExpression(tryExpression, finallyExpression); }
        public static TryCatchFinallyExpression TryCatchFinally(Expression tryExpression, Expression finallyExpression, params CatchBlock[] catchBlocks) { return new TryCatchFinallyExpression(tryExpression, finallyExpression, catchBlocks); }
        public static TryCatchFinallyExpression TryCatch(Expression tryExpression, params CatchBlock[] catchBlocks) { return new TryCatchFinallyExpression(tryExpression, null, catchBlocks); }

        public static LocalVariable LocalVariable(Type variableType, string name) { return new LocalVariable(variableType, name); }
        public static LocalDeclarationExpression DeclareLocal(LocalVariable variable) { return new LocalDeclarationExpression(variable); }
        public static LocalDeclarationExpression DeclareLocal(LocalVariable variable, Expression initialValue) { return new LocalDeclarationExpression(variable, initialValue); }
        public static LocalWriteExpression WriteLocal(LocalVariable variable, Expression value) { return new LocalWriteExpression(variable, value); }
        public static LocalReadExpression ReadLocal(LocalVariable variable) { return new LocalReadExpression(variable); }
        /// <summary>
        /// IfThen expression is always Void type.
        /// It will use Pop if then expression is not Void.
        /// </summary>
        /// <param name="predicate">If non-zero or not-null, then expression would be executed</param>
        /// <param name="thenExpression"></param>
        /// <returns></returns>
        public static IfThenExpression IfThen(Expression predicate, Expression thenExpression) { return new IfThenExpression(predicate, thenExpression); }
        /// <summary>
        /// IfThenElse expression is of thenExpression type.
        /// ThenExpression and elseExpression have to be of the same type.
        /// It is an equivalent of ?: operator
        /// </summary>
        /// <param name="predicate">If non-zero or not-null, then expression would be executed, otherwise elseExpression would be executed.</param>
        /// <param name="thenExpression"></param>
        /// <param name="elseExpression"></param>
        /// <returns></returns>
        public static IfThenElseExpression IfThenElse(Expression predicate, Expression thenExpression, Expression elseExpression) { return new IfThenElseExpression(predicate, thenExpression, elseExpression); }

        public static EmptyExpression Empty() { return new EmptyExpression(); }
        /// <summary>
        /// Returns ~value
        /// </summary>
        public static NotExpression Not(Expression value) { return new NotExpression(value); }
        /// <summary>
        /// Returns -value
        /// </summary>
        public static NegateExpression Negate(Expression value) { return new NegateExpression(value); }

        public static LoopExpression Loop(Expression loop) { return new LoopExpression(loop); }
        public static LoopBreakExpression LoopBreak() { return new LoopBreakExpression(); }
        public static LoopContinueExpression LoopContinue() { return new LoopContinueExpression(); }

        public static BlockExpression Block(params Expression[] expressions) { return new BlockExpression(expressions); }
        public static ValueBlockExpression ValueBlock(Type valueType, params Expression[] expressions) { return new ValueBlockExpression(valueType, expressions); }
        public static AddExpression Add(Expression left, Expression right) { return new AddExpression(left, right); }
        public static AddExpression AddChecked(Expression left, Expression right) { return new AddExpression(left, right, true); }
        public static LessExpression Less(Expression left, Expression right) { return new LessExpression(left, right); }
        public static GreaterExpression Greater(Expression left, Expression right) { return new GreaterExpression(left, right); }
        public static EqualExpression Equal(Expression left, Expression right) { return new EqualExpression(left, right); }
        public static NullExpression Null(Type type) { return new NullExpression(type); }

        public static Expression ReadProperty(Expression instance, PropertyInfo property)
        {
            return Call(instance, property.GetGetMethod(true));
        }

        public static Expression ReadProperty(Expression instance, string propertyName)
        {
            return Call(instance, MemberHelper.FindProperty(instance, propertyName).GetGetMethod(true));
        }

        public static Expression ReadProperty(PropertyInfo property)
        {
            return Call(property.GetGetMethod(true));
        }

        public static ArrayLengthExpression ArrayLength(Expression array) { return new ArrayLengthExpression(array); }
        public static NewArrayExpression NewArray(Type elementType, Expression count) { return new NewArrayExpression(elementType, count); }
        public static ArrayReadExpression ReadArray(Expression array, Expression index) { return new ArrayReadExpression(array, index); }
        public static ArrayWriteExpression WriteArray(Expression array, Expression index, Expression value) { return new ArrayWriteExpression(array, index, value); }
    }
}