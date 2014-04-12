using System;
using System.Reflection;
using CodeBuilder.Expressions;

namespace CodeBuilder.Helpers
{
    public static class Validators
    {
        public static void NullCollectionElementsCheck<T>(T[] array, string paramName) where T : class
        {
            if (array == null)
                throw new ArgumentNullException(paramName);
            for (int i = 0; i < array.Length; ++i)
                if (array[i] == null)
                    throw new ArgumentNullException(string.Format("{0}[{1}]", paramName, i));
        }

        public static T NullCheck<T>(T arg, string paramName) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(paramName);
            return arg;
        }

        public static void AssignableCheck(Type actual, Type toType, string fromToErrorMessage, string paramName)
        {
            NullCheck(actual, paramName);
            if (!toType.IsAssignableFrom(actual))
                throw new ArgumentException(string.Format(fromToErrorMessage, actual, toType), paramName);
        }

        public static void PrimitiveOrClassCheck(Type type, string paramName)
        {
            if (!type.IsPrimitive && !type.IsClass)
                throw new ArgumentException(string.Format("Expected expression type to be primitive or class, got: {0}", type), paramName);
        }

        public static void ParameterCheck(MethodBase methodInfo, Expression[] actual, string paramName)
        {
            var expected = methodInfo.GetParameters();
            if (expected.Length != actual.Length)
                throw new ArgumentException(string.Format("Invalid amount of parameters supplied to method: {0}", methodInfo));

            for (int i = 0; i < expected.Length; ++i)
            {
                if (actual[i] == null)
                    throw new ArgumentNullException(string.Format("values[{0}]", i));
                if (!expected[i].ParameterType.IsAssignableFrom(actual[i].ExpressionType))
                    throw new ArgumentException(string.Format("Parameter expression {0} of type {1} does not match to type: {2}", i, actual[i].ExpressionType, expected[i].ParameterType), paramName);
            }
        }
    }
}
