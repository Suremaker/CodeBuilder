using System;
using System.Reflection;
using CodeBuilder.Expressions;

namespace CodeBuilder.Helpers
{
    public static class Validators
    {
        private static readonly Type[] _intTypes = new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(char), typeof(int), typeof(uint), typeof(long), typeof(ulong) };
        

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

        public static bool IsInHierarchy(Type actual, Type expected)
        {
            //value types have to be boxed or converted explicitly
            if ((actual.IsValueType || expected.IsValueType) && actual != expected)
                return false;
            return expected.IsAssignableFrom(actual);
        }

        public static void HierarchyCheck(Type actual, Type toType, string fromToErrorMessage, string paramName)
        {
            NullCheck(actual, paramName);

            if (!IsInHierarchy(actual, toType))
                throw new ArgumentException(string.Format(fromToErrorMessage, actual, toType), paramName);
        }

        public static void ConversionCheck(Type actual, Type toType, string fromToErrorMessage, string paramName)
        {
            NullCheck(actual, paramName);
            if (actual.IsPrimitive && toType.IsPrimitive)
                return;
            if (!toType.IsAssignableFrom(actual) && !actual.IsAssignableFrom(toType))
                throw new ArgumentException(string.Format(fromToErrorMessage, actual, toType), paramName);
        }

        public static void PrimitiveOrReferenceType(Type type, string paramName)
        {
            if (!type.IsPrimitive && !type.IsClass && !type.IsInterface)
                throw new ArgumentException(string.Format("Expected primitive or reference type, got: {0}", type), paramName);
        }

        public static void ParameterCheck(MethodBase methodInfo, Expression[] actual, string paramName)
        {
            var expected = methodInfo.GetParameters();
            if (expected.Length != actual.Length)
                throw new ArgumentException(string.Format("Invalid amount of parameters supplied to method: {0}", methodInfo));

            for (int i = 0; i < expected.Length; ++i)
            {
                NullCheck(actual[i], string.Format("{0}[{1}]", paramName, i));
                HierarchyCheck(actual[i].ExpressionType, expected[i].ParameterType, "Parameter expression of type {0} does not match to type: {1}", string.Format("{0}[{1}]", paramName, i));
            }
        }

        public static bool IsPrimitiveOrEnum(Type type)
        {
            return type.IsPrimitive || type.IsEnum;
        }

        public static void NumericPrimitiveCheck(Type type, string paramName)
        {
            if (!type.IsPrimitive || type == typeof(bool) || type == typeof(IntPtr) || type == typeof(UIntPtr))
                throw new ArgumentException(string.Format("Expected numeric primitive type, got: {0}", type), paramName);
        }

        public static void ReferenceTypeCheck(Type type, string paramName)
        {
            if (!type.IsClass && !type.IsInterface)
                throw new ArgumentException(string.Format("Expected reference type, got: {0}", type), paramName);
        }

        public static Expression ArrayCheck(Expression array, string paramName)
        {
            NullCheck(array, paramName);
            ArrayCheck(array.ExpressionType,paramName);
            return array;
        }

        public static void ArrayCheck(Type type, string paramName)
        {
            if (!type.IsArray)
                throw new ArgumentException(string.Format("Expected array type, got: {0}", type), paramName);
        }

        public static void IntegralCheck(Type type, string errorMessage, string paramName)
        {
            if (!CollectionHelper.Contains(_intTypes, type))
                throw new ArgumentException(string.Format(errorMessage, paramName));
        }
    }
}
