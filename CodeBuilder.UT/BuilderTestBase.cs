using System;
using System.Reflection.Emit;
using CodeBuilder.Expressions;
using NUnit.Framework;

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

        protected static Action CreateAction(params Expression[] expressions)
        {
            return (Action)CreateMethod(typeof(Action), typeof(void), new Type[0], expressions);
        }

        private static Delegate CreateMethod(Type delegateType, Type returnType, Type[] parameterTypes, params Expression[] expressions)
        {
            var method = new DynamicMethod("testMethod", returnType, parameterTypes, typeof(BuilderTestBase), false);
            new MethodBodyBuilder(method, parameterTypes).AddStatements(expressions).Compile();
            return method.CreateDelegate(delegateType);
        }
    }
}
