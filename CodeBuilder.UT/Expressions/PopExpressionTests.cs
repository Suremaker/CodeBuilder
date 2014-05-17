using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using NUnit.Framework;

namespace CodeBuilder.UT.Expressions
{
    [TestFixture]
    public class PopExpressionTests : BuilderTestBase
    {
        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Pop(Expr.Constant(-1)).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NullParamererTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.Pop(null));
        }

        [Test]
        public void Should_not_allow_void_expression_type()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Pop(Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("Expected expression to be non void type, but got: System.Void"));
        }

        [Test]
        public void Should_pop_value()
        {
            var method = new DynamicMethod("testMethod", typeof(int), new Type[0], typeof(PopExpressionTests), true);
            var generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldc_I4_4);
            Expr.Pop(Expr.Constant(-1)).Compile(new BuildContext(generator, typeof(int), new Type[0], false));
            generator.Emit(OpCodes.Ret);
            var func = (Func<int>)method.CreateDelegate(typeof(Func<int>));

            Assert.That(func(), Is.EqualTo(4));
        }
    }
}