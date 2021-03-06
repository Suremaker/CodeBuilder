using System;
using System.Reflection.Emit;
using CodeBuilder.Expressions;

namespace CodeBuilder.UT
{
    public abstract class BuilderTestBase
    {
        protected static Func<TResult> CreateFunc<TResult>(params Expression[] expressions)
        {
            return (Func<TResult>)CreateMethod(typeof(Func<TResult>), typeof(TResult), new Type[0], expressions);
        }

        protected static Func<TParam, TResult> CreateFunc<TParam, TResult>(params Expression[] expressions)
        {
            return (Func<TParam, TResult>)CreateMethod(typeof(Func<TParam, TResult>), typeof(TResult), new[] { typeof(TParam) }, expressions);
        }

        protected static Func<TParam1, TParam2, TResult> CreateFunc<TParam1, TParam2, TResult>(params Expression[] expressions)
        {
            return (Func<TParam1, TParam2, TResult>)CreateMethod(typeof(Func<TParam1, TParam2, TResult>), typeof(TResult), new[] { typeof(TParam1), typeof(TParam2) }, expressions);
        }

        protected static Func<TParam1, TParam2, TParam3, TResult> CreateFunc<TParam1, TParam2, TParam3, TResult>(params Expression[] expressions)
        {
            return (Func<TParam1, TParam2, TParam3, TResult>)CreateMethod(typeof(Func<TParam1, TParam2, TParam3, TResult>), typeof(TResult), new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) }, expressions);
        }

        protected static Func<TParam1, TParam2, TParam3, TParam4, TResult> CreateFunc<TParam1, TParam2, TParam3, TParam4, TResult>(params Expression[] expressions)
        {
            return (Func<TParam1, TParam2, TParam3, TParam4, TResult>)CreateMethod(typeof(Func<TParam1, TParam2, TParam3, TParam4, TResult>), typeof(TResult), new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) }, expressions);
        }

        protected static Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult> CreateFunc<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>(params Expression[] expressions)
        {
            return (Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>)CreateMethod(typeof(Func<TParam1, TParam2, TParam3, TParam4, TParam5, TResult>), typeof(TResult), new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4), typeof(TParam5) }, expressions);
        }

        protected static Action CreateAction(params Expression[] expressions)
        {
            return (Action)CreateMethod(typeof(Action), typeof(void), new Type[0], expressions);
        }

        protected static Action<TParam> CreateAction<TParam>(params Expression[] expressions)
        {
            return (Action<TParam>)CreateMethod(typeof(Action<TParam>), typeof(void), new[] { typeof(TParam) }, expressions);
        }

        protected static Action<TParam1, TParam2> CreateAction<TParam1, TParam2>(params Expression[] expressions)
        {
            return (Action<TParam1, TParam2>)CreateMethod(typeof(Action<TParam1, TParam2>), typeof(void), new[] { typeof(TParam1), typeof(TParam2) }, expressions);
        }

        protected static Action<TParam1, TParam2, TParam3> CreateAction<TParam1, TParam2, TParam3>(params Expression[] expressions)
        {
            return (Action<TParam1, TParam2, TParam3>)CreateMethod(typeof(Action<TParam1, TParam2, TParam3>), typeof(void), new[] { typeof(TParam1), typeof(TParam2), typeof(TParam3) }, expressions);
        }

        private static Delegate CreateMethod(Type delegateType, Type returnType, Type[] parameterTypes, params Expression[] expressions)
        {
            var method = new DynamicMethod("testMethod", returnType, parameterTypes, typeof(BuilderTestBase), true);
            var builder = new MethodBodyBuilder(method, parameterTypes).AddStatements(expressions);
            Console.WriteLine(builder);
            builder.Compile();
            return method.CreateDelegate(delegateType);
        }
    }
}
