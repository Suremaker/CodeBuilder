using System;
using System.Reflection;
using CodeBuilder.Expressions;

namespace CodeBuilder.Helpers
{
    internal static class MemberHelper
    {
        private const BindingFlags _staticBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags _instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static MethodInfo FindMethod(Expression instance, string methodName, Type[] genericArguments, Expression[] arguments)
        {
            var type = Validators.NullCheck(instance, "instance").ExpressionType;
            return FindMethod(type, methodName, genericArguments, arguments, _instanceBindingFlags);
        }

        public static MethodInfo FindMethod(Type methodOwner, string methodName, Type[] genericArguments, Expression[] arguments)
        {
            return FindMethod(methodOwner, methodName, genericArguments, arguments, _staticBindingFlags);
        }

        public static FieldInfo FindField(Expression instance, string fieldName)
        {
            return Validators.NullCheck(instance, "instance").ExpressionType.GetField(fieldName, _instanceBindingFlags);
        }

        public static PropertyInfo FindProperty(Expression instance, string propertyName)
        {
            return Validators.NullCheck(instance, "instance").ExpressionType.GetProperty(propertyName, _instanceBindingFlags);
        }
        private static MethodInfo FindMethod(Type methodOwner, string methodName, Type[] genericArguments, Expression[] arguments, BindingFlags bindingFlags)
        {
            if (genericArguments == null || genericArguments.Length == 0)
                return FindNonGenericMethodInfo(methodOwner, methodName, arguments, bindingFlags);
            return FindGenericMethodInfo(methodOwner, methodName, genericArguments, arguments, bindingFlags);
        }

        private static MethodInfo FindGenericMethodInfo(Type methodOwner, string methodName, Type[] genericArguments, Expression[] arguments, BindingFlags bindingFlags)
        {
            foreach (var method in methodOwner.GetMethods(bindingFlags))
            {
                if (!method.IsGenericMethodDefinition) continue;
                if (!method.Name.Equals(methodName, StringComparison.Ordinal)) continue;
                if (method.GetGenericArguments().Length != genericArguments.Length) continue;
                if (AreParametersMatching(method.GetParameters(), genericArguments, arguments))
                    return method.MakeGenericMethod(genericArguments);
            }
            return null;
        }

        private static bool AreParametersMatching(ParameterInfo[] methodParameters, Type[] genericArguments, Expression[] arguments)
        {
            if (methodParameters.Length != arguments.Length)
                return false;
            for (int i = 0; i < methodParameters.Length; ++i)
            {
                var parameterType = methodParameters[i].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    if (genericArguments[parameterType.GenericParameterPosition] != arguments[i].ExpressionType)
                        return false;
                }
                else
                {
                    if (parameterType != arguments[i].ExpressionType)
                        return false;
                }
            }
            return true;
        }

        private static MethodInfo FindNonGenericMethodInfo(Type methodOwner, string methodName, Expression[] arguments, BindingFlags bindingFlags)
        {
            Validators.NullCollectionElementsCheck(arguments, "arguments");

            var argumentTypes = new Type[arguments.Length];
            for (int i = 0; i < arguments.Length; i += 1)
                argumentTypes[i] = arguments[i].ExpressionType;

            return methodOwner.GetMethod(methodName,
                bindingFlags,
                null, argumentTypes, null);
        }

    }
}
